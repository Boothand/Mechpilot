//using System.Collections;
using UnityEngine;

public class Collidable : MechComponent
{
	public delegate void Collide(Collider col);
	public event Collide OnCollision;

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

	protected virtual void OnTriggerEnter(Collider col)
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

	//void OnCollisionEnter(Collision col)
	//{
	//	if (/*col.transform.root != transform.root &&*/
	//		IsInLayerMask(col.gameObject, layerMask))
	//	{
	//		print("Calling event, hit " + col.transform.name);

	//		if (OnCollision != null)
	//		{
	//			OnCollision(col);
	//		}
	//	}
	//}

	void Update()
	{
		
	}
}