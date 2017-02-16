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

		//switch (aiCombat.combatState)
		//{
		//	case AI_Combat.CombatState.Defend:

		//		switch (aiCombat.blockMethod)
		//		{
		//			case AI_Combat.BlockMethod.Confident:

		//				if (!FacingTarget(enemy.transform.position, 0.995f))
		//				{
		//					if (IsWithinCombatDistance())
		//					{
		//						input.moveVert = -1f;
		//					}
		//					else
		//					{
		//						input.moveVert = 1f;
		//					}
		//				}

		//				break;

		//			case AI_Combat.BlockMethod.LowHealth:

		//				break;

		//			case AI_Combat.BlockMethod.LowStamina:

		//				break;
		//		}
		//		break;

		//	case AI_Combat.CombatState.Attack:

		//		switch (aiCombat.attackMethod)
		//		{
		//			case AI_Combat.AttackMethod.Aggressive:

		//				break;

		//			case AI_Combat.AttackMethod.Counter:

		//				break;

		//			case AI_Combat.AttackMethod.Impatient:

		//				break;
		//		}

		//		//Walk to him
		//		if (!CanSwingAtEnemy(enemy.transform))
		//		{
		//			WalkForward();
		//		}

		//		break;
		//}
	}
}