﻿using System.Collections;
using UnityEngine;

public class AI_Attacker : AI_MechComponent
{
	Vector3 localHandBasePos;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		StartCoroutine(AttackRoutine());
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
		float limit = arms.armControl.getIdleRotationLimit;
		float randomAngle = 0f + Random.Range(-limit, limit);

		return randomAngle;
	}

	void MoveHandsToPos(Vector3 localPos)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

		Vector3 dir = localPos - rIK.localPosition;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	void MoveHandsInDirection(Vector3 dir)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

		Vector3 direction = dir;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	void RotateHandsToAngle(float angle)
	{
		float aimAngle = arms.armControl.sideTargetAngle;

		if (aimAngle < angle - 0.01f)
		{
			input.rArmRot = 1f;
		}
		else if (aimAngle > angle + 0.01f)
		{
			input.rArmRot = -1f;
		}
	}

	void StopArms()
	{
		input.rArmHorz = 0f;
		input.rArmVert = 0f;
	}

	void StopHandRotation()
	{
		input.rArmRot = 0f;
	}

	IEnumerator AttackRoutine()
	{
		while (true)
		{
			input.attack = true;

			Vector3 attackPos = DecideWindupPosition();
			float attackAngle = DecideWindupRotation();

			float aTimer = 0f;
			while (arms.armControl.state != ArmControl.State.WindedUp)
			{
				aTimer += Time.deltaTime;
				MoveHandsToPos(attackPos);
				RotateHandsToAngle(attackAngle);
				yield return null;
			}

			//StopArms();
			StopHandRotation();

			yield return new WaitForSeconds(0.2f);

			input.attack = false;

			Vector3 dir = localHandBasePos - attackPos;

			while (arms.armControl.state != ArmControl.State.Defend)
			{
				MoveHandsInDirection(dir);
				yield return null;
			}


			StopArms();

			yield return new WaitForSeconds(0.3f);
		}
	}

	void Update()
	{
		Vector3 basePos = arms.armControl.handCenterPos;
		localHandBasePos = mech.transform.InverseTransformPoint(basePos);
	}
}