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

		float rotDir = 1f;

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{
			aiCombat.CrossEnemySwordDir(rotDir);
		}

		print("In low health block");
	}

	protected override void Update()
	{
		base.Update();
	}
}