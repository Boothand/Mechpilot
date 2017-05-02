//using System.Collections;
using UnityEngine;

public class FaceOpponentCamera : MechComponent
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		//Face the opponent's camera
		if (mech.tempEnemy)
		{
			transform.forward = mech.tempEnemy.pilot.cameraFollow.transform.position - transform.position;
		}
	}
}