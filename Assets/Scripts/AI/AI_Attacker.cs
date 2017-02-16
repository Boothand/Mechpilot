using System.Collections;
using UnityEngine;

public class AI_Attacker : AI_MechComponent
{
	bool inAttackRoutine;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{

	}

	Vector3 DecideWindupPosition()
	{
		Vector3 windupPos = aiCombat.localHandBasePos;
		float range = 0.5f;
		windupPos.x += Random.Range(-range, range);
		windupPos.y += Random.Range(-range, range);

		return windupPos;
	}

	float DecideWindupRotation()
	{
		float limit = arms.armBlockState.getSideRotationLimit;
		float randomAngle = 0f + Random.Range(-limit, limit);

		return randomAngle;
	}

	IEnumerator AttackRoutine()
	{
		inAttackRoutine = true;
		input.attack = true;

		Vector3 attackPos = DecideWindupPosition();
		float attackAngle = DecideWindupRotation();

		float aTimer = 0f;
		while (arms.armControl.state != ArmControl.State.WindedUp)
		{
			aTimer += Time.deltaTime;
			aiCombat.MoveHandsToPos(attackPos);
			aiCombat.RotateHandsToAngle(attackAngle);
			yield return null;
		}

		//StopArms();
		aiCombat.StopHandRotation();

		yield return new WaitForSeconds(0.2f);

		input.attack = false;

		Vector3 dir = aiCombat.localHandBasePos - attackPos;

		while (arms.armControl.state != ArmControl.State.Defend)
		{
			aiCombat.MoveHandsInDirection(dir);
			yield return null;
		}

		aiCombat.StopArms();

		yield return new WaitForSeconds(0.3f);
		inAttackRoutine = false;
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
}