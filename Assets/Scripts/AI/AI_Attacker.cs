using System.Collections;
using UnityEngine;

public class AI_Attacker : AI_MechComponent
{
#if LEGACY
	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{

	}

	
	protected override void Update()
	{
		base.Update();

		//if (aiCombat.combatState == AI_Combat.CombatState.Attack)
		//{
		//	switch (aiCombat.attackMethod)
		//	{
		//		case AI_Combat.AttackMethod.Aggressive:

		//			if (!inAttackRoutine &&
		//				CanSwingAtEnemy(enemy.transform))
		//			{
		//				StopAllCoroutines();
		//				StartCoroutine(AttackRoutine());
		//			}

		//			if (!CanSwingAtEnemy(enemy.transform))
		//			{
		//				aiCombat.MoveHandsToPos(aiCombat.localHandBasePos);
		//			}
		//			break;

		//		case AI_Combat.AttackMethod.Counter:

		//			break;

		//		case AI_Combat.AttackMethod.Impatient:

		//			break;
		//	}
		//}
	}
#endif
}