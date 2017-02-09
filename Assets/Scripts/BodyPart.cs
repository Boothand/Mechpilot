//using System.Collections;
using UnityEngine;

public class BodyPart : Collidable
{


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
				print(transform.name + " got hit by a sword");
			}
		}
	}

	void Update()
	{
		
	}
}