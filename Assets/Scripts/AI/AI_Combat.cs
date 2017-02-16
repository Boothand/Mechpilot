using System.Collections;
using UnityEngine;

public class AI_Combat : AI_MechComponent
{
	[SerializeField] float combatDistance = 45f;
	[SerializeField] float impatienceTime = 3f;
	float impatienceTimer;
	public AI_AttackMethod activeAttackMethod { get; private set; }
	public AI_BlockMethod activeBlockMethod { get; private set; }

	[SerializeField] AI_AttackMethod aggressiveAttack, counterAttack, impatientAttack;
	[SerializeField] AI_BlockMethod confidentBlock, lowStaminaBlock, lowHealthBlock;

	public enum CombatState { Defend, Attack, Idle }
	public CombatState combatState { get; private set; }

	public float getCombatDistance { get { return combatDistance; } }

	public Vector3 localHandBasePos { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();

		combatState = CombatState.Attack;
	}

	#region Helper functions
	public void MoveHandsToPos(Vector3 localPos)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

		Vector3 dir = localPos - rIK.localPosition;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	public void MoveHandsInDirection(Vector3 dir)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

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

	public bool LowStamina(Mech someMech)
	{
		return someMech.weaponsOfficer.energyManager.stamina < 40f;
	}

	public bool LowHealth(Mech someMech)
	{
		return someMech.weaponsOfficer.healthManager.getHealth < 40f;
	}

	public bool CanSeeEnemy()
	{
		return true; //For now...
	}
	#endregion

	void DecideAttackAndBlockMethod()
	{
		//Defaults:
		activeAttackMethod = aggressiveAttack;
		activeBlockMethod = confidentBlock;

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
		DecideAttackAndBlockMethod();

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