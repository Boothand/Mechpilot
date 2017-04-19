//using System.Collections;
using UnityEngine;

public class AI_LowStaminaBlock : AI_BlockMethod
{
#if LEGACY

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		print("In low stamina block");

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{
			aiCombat.CrossEnemySwordDir();
		}

		if (aiCombat.LowHealth(mech))
		{
			aiCombat.SetBlockMethod(aiCombat.getLowHealthBlock);
		}

		if (energyManager.stamina > 60f)
		{
			aiCombat.SetAttackMethod(aiCombat.getAggressiveAttack);
		}
	}

	protected override void Update()
	{
		base.Update();
	}
#endif
}