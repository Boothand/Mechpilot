using System.Collections;
using UnityEngine;

public class AI_AggressiveAttack : AI_AttackMethod
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}


	public override void RunComponent()
	{
		base.RunComponent();

		print("In aggressive");

		aiCombat.TurnHeadTowards(enemy.transform.position);

		if (!aiCombat.inAttackRoutine &&
			CanSwingAtEnemy(enemy.transform))
		{
			aiCombat.StopAllCoroutines();
			aiCombat.StartCoroutine(aiCombat.AttackRoutine());
		}

		if (!CanSwingAtEnemy(enemy.transform))
		{
			aiCombat.MoveHandsToPos(aiCombat.localHandBasePos);
			aiCombat.RotateHandsToAngle(0f);
			aiCombat.WalkTo(enemy.transform.position);
		}

		if (aiCombat.LowStamina(mech, 45f))
		{
			aiCombat.combatState = AI_Combat.CombatState.Defend;
		}
	}
}