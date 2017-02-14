using System.Collections;
using UnityEngine;

public class AI_Movement : AI_MechComponent
{
	[SerializeField] float turnHeadSpeed = 1f;

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

	void WalkForward()
	{
		input.moveVert = 1f;
	}

	protected override void Update()
	{
		base.Update();

		input.moveVert = 0f;
		input.moveHorz = 0f;
		input.lookHorz = 0f;

		//Look at the player
		TurnHeadTowards(enemy.transform.position);

		switch (aiCombat.combatState)
		{
			case AI_Combat.CombatState.Defend:

				break;

			case AI_Combat.CombatState.Attack:

				//Walk to him
				if (!CanSwingAtEnemy(enemy.transform))
				{
					WalkForward();
				}

				break;
		}
	}
}