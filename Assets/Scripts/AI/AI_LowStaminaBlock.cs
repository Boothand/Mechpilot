//using System.Collections;
using UnityEngine;

public class AI_LowStaminaBlock : AI_BlockMethod
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		print("In low stamina block");

		float rotDir = 1f;

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{
			aiCombat.CrossEnemySwordDir(rotDir);
		}
	}

	protected override void Update()
	{
		base.Update();
	}
}