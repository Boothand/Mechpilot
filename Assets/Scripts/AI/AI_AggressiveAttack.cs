using System.Collections;
using UnityEngine;

public class AI_AggressiveAttack : AI_AttackMethod
{
	float patienceTimer;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		print("In aggressive");

		aiCombat.TurnHeadTowards(enemy.transform.position);

		//If he's not attacking me, just attack him (if I can)...
		if (!aiCombat.LowStamina(mech, 20f) &&
			!aiCombat.inAttackRoutine &&
			CanSwingAtEnemy(enemy.transform) &&
			aiCombat.GetState(enemy) != ArmControl.State.WindedUp &&
			aiCombat.GetState(enemy) != ArmControl.State.Attack)
		{
			aiCombat.StopAllCoroutines();
			aiCombat.StartCoroutine(aiCombat.AttackRoutine());
		}

		//If I'm not in a position to attack him, center my hands, move towards him.
		if (!CanSwingAtEnemy(enemy.transform))
		{
			aiCombat.MoveHandsToPos(aiCombat.localHandBasePos);
			aiCombat.RotateHandsToAngle(0f);
			aiCombat.WalkTo(enemy.transform.position);
		}
		
		//Just block if I'm low on stamina.
		if (aiCombat.LowStamina(mech))
		{
			aiCombat.SetBlockMethod(aiCombat.getLowStaminaBlock);
		}

		//If he attacks or is probably about to, block.
		if (aiCombat.GetState(enemy) == ArmControl.State.WindedUp ||
			aiCombat.GetState(enemy) == ArmControl.State.Attack)
		{
			aiCombat.SetBlockMethod(aiCombat.getConfidentBlock);
		}
	}
}