//using System.Collections;
using UnityEngine;

public class Collidable : MechComponent
{
	public delegate void CollisionEvent(Collision col);
	public delegate void TriggerEvent(Collider col);
	public event CollisionEvent OnCollisionEnterEvent;
	public event TriggerEvent OnTriggerEnterEvent;

	[SerializeField] protected LayerMask layerMask;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	bool IsInLayerMask(GameObject obj, LayerMask mask)
	{
		int bit = (1 << obj.layer);

		if ((mask.value & bit) > 0)
		{
			return true;
		}

		return false;
	}

	protected bool IsValid(GameObject obj)
	{
		return obj.transform.root != transform.root &&
			IsInLayerMask(obj, layerMask);
	}

	protected virtual void RunTriggerEvent(Collider col)
	{
		if (OnTriggerEnterEvent != null)
		{
			OnTriggerEnterEvent(col);
		}
	}

	protected virtual void RunCollisionEvent(Collision col)
	{
		if (OnCollisionEnterEvent != null)
		{
			OnCollisionEnterEvent(col);
		}
	}

	protected virtual void OnCollisionEnter(Collision col)
	{
		if (IsValid(col.gameObject))
		{
			RunCollisionEvent(col);
		}
	}

	protected virtual void OnTriggerEnter(Collider col)
	{
		if (IsValid(col.gameObject))
		{
			RunTriggerEvent(col);
		}
	}

	void Update()
	{
		
	}
}