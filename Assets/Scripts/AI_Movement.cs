using System.Collections;
using UnityEngine;

public class AI_Movement : AI_MechComponent
{
	[SerializeField] float turnHeadSpeed = 1f;
	[SerializeField] float combatDistance = 1.5f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void TurnHeadTowards(Vector3 target)
	{
		Vector3 dirToTarget = target - mech.transform.position;
		dirToTarget.Normalize();

		float rightDot = Vector3.Dot(hierarchy.head.transform.right, dirToTarget);		
		
		if (rightDot > 0f)
		{
			input.lookHorz = turnHeadSpeed;
		}

		if (rightDot < 0f)
		{
			input.lookHorz = -turnHeadSpeed;
		}
	}

	bool FacingTarget(Vector3 target)
	{
		if (enemyFrontDot > 0.95f)
		{
			return true;
		}

		return false;
	}

	void WalkForward()
	{
		input.moveVert = 1f;
	}

	void Update()
	{
		input.moveVert = 0f;
		input.moveHorz = 0f;
		input.lookHorz = 0f;

		//Look at the player
		TurnHeadTowards(enemy.transform.position);

		//Walk to him
		if (distanceToEnemy > combatDistance ||
			!FacingTarget(enemy.transform.position))
		{
			WalkForward();
		}
	}
}