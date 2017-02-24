//using System.Collections;
using UnityEngine;

public class AI_ConfidentBlock : AI_BlockMethod
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		print("In confident block");

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{
			aiCombat.CrossEnemySwordDir();
		}

		if (aiCombat.GetState(enemy) != ArmControl.State.WindUp &&
			aiCombat.GetState(enemy) != ArmControl.State.WindedUp &&
			aiCombat.GetState(enemy) != ArmControl.State.Attack)
		{
			if (!aiCombat.LowStamina(mech) && !aiCombat.LowHealth(mech))
			{
				aiCombat.SetAttackMethod(aiCombat.getAggressiveAttack);
			}

			if (aiCombat.LowHealth(mech))
			{
				aiCombat.SetBlockMethod(aiCombat.getLowHealthBlock);
			}

			if (aiCombat.LowStamina(mech))
			{
				aiCombat.SetBlockMethod(aiCombat.getLowStaminaBlock);
			}
		}

		//Distance management
		if (IsWithinSwingDistance())
		{
			aiCombat.WalkBackward();
		}

		if (!IsWithinCombatDistance())
		{
			aiCombat.WalkForward();
		}
	}

	protected override void Update()
	{
		base.Update();
	}
}