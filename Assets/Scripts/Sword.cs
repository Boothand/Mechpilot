//using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
	[SerializeField] Transform swordTip;
	public Transform getSwordTip { get { return swordTip; } }
	public Vector3 swordTipVelocity { get; private set; }

	Vector3 lastPos;

	public delegate void Collide(Collision col);
	public event Collide OnCollision;

	[SerializeField]
	LayerMask layerMask;

	void Awake()
	{
		
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
		swordTipVelocity = swordTip.position - lastPos;

		lastPos = swordTip.position;
	}
}