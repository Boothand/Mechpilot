using System.Collections;
using UnityEngine;

public class AI_Combat : AI_MechComponent
{
	[SerializeField] float combatDistance = 45f;
	[SerializeField] float swingDistance = 32f;
	[SerializeField] float impatienceTime = 3f;
	[SerializeField] float turnHeadSpeed = 1f;
	[SerializeField] float posUnderEnemyArmPos = -2f;
	float impatienceTimer;
	public bool inAttackRoutine { get; private set; }

	public AI_AttackMethod activeAttackMethod { get; private set; }
	public AI_BlockMethod activeBlockMethod { get; private set; }

	[SerializeField] AI_AttackMethod aggressiveAttack, counterAttack, impatientAttack;
	[SerializeField] AI_BlockMethod confidentBlock, lowStaminaBlock, lowHealthBlock;

	public AI_AttackMethod getAggressiveAttack { get { return aggressiveAttack; } }
	public AI_AttackMethod getCounterAttack { get { return counterAttack; } }
	public AI_AttackMethod getImpatientAttack { get { return impatientAttack; } }
	public AI_BlockMethod getConfidentBlock { get { return confidentBlock; } }
	public AI_BlockMethod getLowStaminaBlock { get { return lowStaminaBlock; } }
	public AI_BlockMethod getLowHealthBlock { get { return lowHealthBlock; } }

	public enum CombatState { Defend, Attack, Idle }
	public CombatState combatState { get; set; }

	public float getCombatDistance { get { return combatDistance; } }
	public float getSwingDistance { get { return swingDistance; } }

	public Vector3 localHandBasePos { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();

		combatState = CombatState.Attack;
		activeAttackMethod = aggressiveAttack;
		activeBlockMethod = confidentBlock;
	}

	#region Helper functions
	public void MoveHandsToPos(Vector3 localPos)
	{
		Transform rIK = arms.getRhandIKTarget;

		Vector3 dir = localPos - rIK.localPosition;
		input.rArmHorz = dir.x * 8f;
		input.rArmVert = dir.y * 8f;
	}

	public void MoveHandsInDirection(Vector3 dir)
	{
		Transform rIK = arms.getRhandIKTarget;

		Vector3 direction = dir;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	public void MoveHandsToSidePos(float pos)
	{
		if (arms.armBlockState.rArmPos.x < pos - 0.1f)
		{
			input.rArmHorz = 1f;
		}
		else if (arms.armBlockState.rArmPos.x > pos + 0.1f)
		{
			input.rArmHorz = -1f;
		}
	}

	public void RotateHandsToAngle(float angle)
	{
		float aimAngle = arms.armBlockState.sideTargetAngle;

		if (aimAngle < angle - 0.01f)
		{
			input.rArmRot = 1f;
		}
		else if (aimAngle > angle + 0.01f)
		{
			input.rArmRot = -1f;
		}
	}

	public void TurnHeadTowards(Vector3 target)
	{
		Vector3 dirToTarget = target - mech.transform.position;
		dirToTarget.Normalize();

		float rightDot = Vector3.Dot(hierarchy.head.transform.right, dirToTarget);

		if (rightDot > 0f)
		{
			input.lookHorz = turnHeadSpeed;
		}

		if (rightDot < 0f)
		{
			input.lookHorz = -turnHeadSpeed;
		}
	}

	public void StopArms()
	{
		input.rArmHorz = 0f;
		input.rArmVert = 0f;
	}

	public void StopHandRotation()
	{
		input.rArmRot = 0f;
	}

	public bool OpponentIsWindingUp(Mech otherMech)
	{
		switch (otherMech.weaponsOfficer.armControl.state)
		{
			case ArmControl.State.WindUp:
			case ArmControl.State.WindedUp:
				return true;
		}

		return false;
	}

	public bool OpponentIsAttacking(Mech otherMech)
	{
		switch (otherMech.weaponsOfficer.armControl.state)
		{
			case ArmControl.State.Attack:
				return true;
		}

		return false;
	}

	public ArmControl.State GetState(Mech otherMech)
	{
		return otherMech.weaponsOfficer.armControl.state;
	}

	public bool OpponentIsDefending(Mech otherMech)
	{
		switch (otherMech.weaponsOfficer.armControl.state)
		{
			case ArmControl.State.Defend:
				return true;
		}

		return false;
	}

	public bool LowStamina(Mech someMech, float threshold = 40f)
	{
		return someMech.weaponsOfficer.energyManager.stamina < threshold;
	}

	public bool LowHealth(Mech someMech, float threshold = 40f)
	{
		return someMech.weaponsOfficer.healthManager.getHealth < threshold;
	}

	public bool CanSeeEnemy()
	{
		return true; //For now...
	}

	public Vector3 DecideWindupPosition()
	{
		Vector3 windupPos = aiCombat.localHandBasePos;
		float range = 0.5f;
		windupPos.x += Random.Range(-range, range);
		windupPos.y += Random.Range(-range, range);

		return windupPos;
	}

	public float DecideWindupRotation()
	{
		float limit = arms.armBlockState.getSideRotationLimit;
		float randomAngle = 0f + Random.Range(-limit, limit);

		return randomAngle;
	}

	public void WalkForward()
	{
		input.moveVert = 1f;
	}

	public void WalkBackward()
	{
		input.moveVert = -1f;
	}

	public void MoveSideways(float dir = 1f)
	{
		input.moveHorz = dir;
	}

	public void WalkTo(Vector3 pos)
	{
		TurnHeadTowards(pos);
		WalkForward();
	}

	public void CrossEnemySwordDir()
	{
		float rotDir = 1f;
		Sword enemySword = enemy.weaponsOfficer.getWeapon;
		Vector3 enemyTipsPos = enemySword.getSwordTip.position;
		Vector3 enemySwordDir = -enemySword.transform.right;

		float enemyTargetAngle = enemy.weaponsOfficer.armBlockState.sideTargetAngle;

		aiCombat.MoveHandsToSidePos(-5f);

		if (enemyTargetAngle < 0f)
		{
			rotDir = -rotDir;
			aiCombat.MoveHandsToSidePos(5f);
		}

		if (Mathf.Abs(enemyTargetAngle) > 80f)
		{
			input.rArmVert = -1f;
		}
		else
		{
			float enemyPos = enemy.weaponsOfficer.armBlockState.rArmPos.y;
			float underEnemyPos = enemyPos - posUnderEnemyArmPos;
			float asd = Mathf.Lerp(enemyPos, underEnemyPos, Mathf.Abs(enemyTargetAngle) / 80f);
			//print(arms.armBlockState.rArmPos.y + " " + asd);
			if (arms.armBlockState.rArmPos.y > asd)
			{
				input.rArmVert = -1f;
			}
			else
			{
				input.rArmVert = 1f;
			}
			//aiCombat.MoveHandsToPos(aiCombat.localHandBasePos);
		}

		float dirDot = Vector3.Dot(-arms.getWeapon.transform.right, enemySwordDir);
		//print(dirDot);
		if (dirDot > 0.4f)
		{
			input.rArmRot = rotDir;
		}
		else if (dirDot < 0.3f)
		{
			input.rArmRot = -rotDir;
		}
	}

	public void ZeroAllInputs()
	{
		input.rArmHorz = 0f;
		input.rArmVert = 0f;
		input.rArmRot = 0f;
		input.lookHorz = 0f;
		input.lookVert = 0f;
		input.moveHorz = 0f;
		input.moveVert = 0f;
	}

	#endregion

	public IEnumerator AttackRoutine()
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

			if (!IsWithinSwingDistance())
			{
				WalkForward();
			}

			yield return null;
		}

		aiCombat.StopArms();

		yield return new WaitForSeconds(0.3f);
		inAttackRoutine = false;
	}

	void DecideAttackAndBlockMethod()
	{
		//Defaults:
		if (energyManager.stamina > 60f)
		{
			activeAttackMethod = aggressiveAttack;
			activeBlockMethod = confidentBlock;
		}

		//If I'm healthy and got stamina, I'll be agressive. Might wind-up and attack, and dash away if he's attacking.
		//Might kick if he's just winding up and standing there.
		//Might try to block when he winds up, but I'll do the above if I grow impatient.


		//If my stamina is low, I'll block. If I need to attack (for some reason), only counter method.
		if (LowStamina(mech))
		{
			activeBlockMethod = lowStaminaBlock;
			activeAttackMethod = counterAttack;
		}

		//If my health is low, I'll block more and rather try to counter his attacks.
		if (LowHealth(mech))
		{
			activeBlockMethod = lowHealthBlock;
			activeAttackMethod = counterAttack;
		}

		if (GetState(enemy) == ArmControl.State.WindedUp)
		{
			impatienceTimer += Time.deltaTime;
		}
		if (GetState(enemy) == ArmControl.State.Attack)
		{
			impatienceTimer = 0f;
		}

		if (!LowHealth(mech) && !LowStamina(mech) &&
			impatienceTimer > impatienceTime)
		{
			activeAttackMethod = impatientAttack;
			activeBlockMethod = confidentBlock;
		}
	}

	public void SetAttackMethod(AI_AttackMethod method)
	{
		combatState = CombatState.Attack;
		activeAttackMethod = method;
	}

	public void SetBlockMethod(AI_BlockMethod method)
	{
		combatState = CombatState.Defend;
		activeBlockMethod = method;
	}

	protected override void Update()
	{
		base.Update();

		Vector3 basePos = arms.armControl.handCenterPos;
		localHandBasePos = mech.transform.InverseTransformPoint(basePos);

		if (!CanSeeEnemy())
		{
			combatState = CombatState.Idle;
			return;
		}

		//Decide which block and attack method to use
		//DecideAttackAndBlockMethod();

		if (combatState == CombatState.Attack)
		{
			activeAttackMethod.RunComponent();
		}
		else if (combatState == CombatState.Defend)
		{
			activeBlockMethod.RunComponent();
		}
	}
}