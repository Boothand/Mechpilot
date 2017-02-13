//using System.Collections;
using UnityEngine;

public class AI_Defender : AI_MechComponent
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void Update()
	{
		base.Update();

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{

		}
	}
}