using System.Collections;
using UnityEngine;

public class AI_Attacker : AI_MechComponent
{
	Vector3 localBasePos;

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
		localBasePos.x += Random.Range(-range, range);
		localBasePos.y += Random.Range(-range, range);

		return localBasePos;
	}

	float DecideWindupRotation()
	{
		float limit = arms.armRotation.getIdleRotationLimit;
		float randomAngle = 0f + Random.Range(-limit, limit);

		return randomAngle;
	}

	void MoveHandsToPos(Vector3 localPos)
	{
		Transform rIK = arms.armMovement.rHandIK;

		Vector3 dir = localPos - rIK.localPosition;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	void MoveHandsInDirection(Vector3 dir)
	{
		Transform rIK = arms.armMovement.rHandIK;

		Vector3 direction = dir;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	void RotateHandsToAngle(float angle)
	{
		float aimAngle = arms.armRotation.idleTargetAngle;

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

			while (arms.armRotation.state != ArmRotation.State.WindedUp)
			{
				MoveHandsToPos(attackPos);
				RotateHandsToAngle(attackAngle);
				yield return null;
			}

			//StopArms();
			StopHandRotation();

			yield return new WaitForSeconds(0.2f);

			input.attack = false;

			Vector3 dir = localBasePos - attackPos;

			while (arms.armRotation.state != ArmRotation.State.Defend)
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
		Vector3 basePos = arms.armMovement.handCenterPos;
		localBasePos = mech.transform.InverseTransformPoint(basePos);

		//Debug.DrawRay(hierarchy.rhand.position, hierarchy.rhand.up, Color.red);

		Debug.DrawRay(hierarchy.head.position, hierarchy.head.forward, Color.red);

		Vector3 dirToEnemy = transform.position - enemy.transform.position;
		dirToEnemy.Normalize();

	}
}