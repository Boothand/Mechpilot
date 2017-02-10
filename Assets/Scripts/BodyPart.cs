//using System.Collections;
using UnityEngine;

public class BodyPart : Collidable
{
	public enum BodyGroup { Head, Body, Arms, Legs, NumBodyGroups }	
	[SerializeField] BodyGroup bodyGroup;



	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnTriggerEnter(Collider col)
	{
		base.OnTriggerEnter(col);

		if (!IsValid(col.gameObject))
			return;

		Sword swordHittingMe = col.GetComponent<Sword>();

		if (swordHittingMe)
		{
			if (swordHittingMe.arms.armControl.prevState == ArmControl.State.Attack)
			{
				healthManager.GetHit(bodyGroup);
			}
		}
	}

	void Update()
	{
		
	}
}