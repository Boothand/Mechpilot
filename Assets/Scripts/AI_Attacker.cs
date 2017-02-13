using System.Collections;
using UnityEngine;

public class AI_Attacker : AI_MechComponent
{
	Vector3 localHandBasePos;
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
		float range = 0.5f;
		localHandBasePos.x += Random.Range(-range, range);
		localHandBasePos.y += Random.Range(-range, range);

		return localHandBasePos;
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

		Vector3 dir = localHandBasePos - attackPos;

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
		
		Vector3 basePos = arms.armControl.handCenterPos;
		localHandBasePos = mech.transform.InverseTransformPoint(basePos);

		if (aiCombat.combatState == AI_Combat.CombatState.Attack)
		{
			if (!inAttackRoutine &&
				CanSwingAtEnemy(enemy.transform))
			{
				StartCoroutine(AttackRoutine());
			}

			if (!CanSwingAtEnemy(enemy.transform))
			{
				aiCombat.MoveHandsToPos(localHandBasePos);
			}
		}
	}
}