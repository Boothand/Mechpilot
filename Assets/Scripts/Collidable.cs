//using System.Collections;
using UnityEngine;

public class Collidable : MechComponent
{
	public System.Action<Collision> OnCollisionEnterEvent;
	public System.Action<Collider> OnTriggerEnterEvent;

	[SerializeField] protected LayerMask layerMask;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	//Checks if a given object's layer is in a layermask, might be unnecessary
	//because of Unity's collision matrix though..
	bool IsInLayerMask(GameObject obj, LayerMask mask)
	{
		int bit = (1 << obj.layer);

		if ((mask.value & bit) > 0)
		{
			return true;
		}

		return false;
	}

	//Filter out unwanted collisions
	protected bool IsValid(GameObject obj)
	{
		return obj.transform.root != transform.root &&
			IsInLayerMask(obj, layerMask);
	}

	//Can be overridden to do additional things on trigger enter
	protected virtual void RunTriggerEvent(Collider col)
	{
		if (OnTriggerEnterEvent != null)
		{
			OnTriggerEnterEvent(col);
		}
	}

	//Can be overridden to do additional things on collision enter
	protected virtual void RunCollisionEvent(Collision col)
	{
		if (OnCollisionEnterEvent != null)
		{
			OnCollisionEnterEvent(col);
		}
	}

	//Should not be overridden
	protected void OnCollisionEnter(Collision col)
	{
		if (IsValid(col.gameObject))
		{
			RunCollisionEvent(col);
		}
	}

	//Should not be overridden
	protected void OnTriggerEnter(Collider col)
	{
		if (IsValid(col.gameObject))
		{
			RunTriggerEvent(col);
		}
	}
}