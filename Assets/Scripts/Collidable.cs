//using System.Collections;
using UnityEngine;

public class Collidable : MechComponent
{
	public delegate void Collide(Collision col);
	public event Collide OnCollision;

	[SerializeField] LayerMask layerMask;

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

	void OnTriggerEnter(Collider col)
	{
		if (col.transform.root != transform.root &&
			IsInLayerMask(col.gameObject, layerMask))
		{

			print("Collided with " + col.name);
			if (OnCollision != null)
			{
				//OnCollision(col);
			}
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.transform.root != transform.root &&
			IsInLayerMask(col.gameObject, layerMask))
		{
			if (OnCollision != null)
			{
				OnCollision(col);
			}
		}
	}

	void Update()
	{
		
	}
}