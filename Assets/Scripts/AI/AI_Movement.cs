using System.Collections;
using UnityEngine;

public class AI_Movement : AI_MechComponent
{

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void Update()
	{
		base.Update();

		//input.moveVert = 0f;
		//input.moveHorz = 0f;
		//input.lookHorz = 0f;

		//Look at the player
		//TurnHeadTowards(enemy.transform.position);

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