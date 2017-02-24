//using System.Collections;
using UnityEngine;

public class AI_LowHealthBlock : AI_BlockMethod
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{
			aiCombat.CrossEnemySwordDir();
		}

		print("In low health block");
	}

	protected override void Update()
	{
		base.Update();
	}
}