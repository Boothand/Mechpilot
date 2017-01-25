using System.Collections;
using UnityEngine;

public class AI_Attacker : MechComponent
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

	void MoveHandsToPos(Vector3 localPos)
	{
		Transform rIK = arms.armMovement.rHandIK;

		Vector3 dir = localPos - rIK.localPosition;

		input.rArmHorz += Mathf.Sign(dir.x);
		input.rArmVert += Mathf.Sign(dir.y);
	}

	IEnumerator AttackRoutine()
	{
		while (true)
		{
			input.attack = true;

			Vector3 attackPos = DecideWindupPosition();

			while (arms.weaponControl.state != ArmRotation.State.WindedUp)
			{
				MoveHandsToPos(attackPos);
				yield return null;
			}

			yield return new WaitForSeconds(0.5f);

			input.attack = false;

			while (arms.weaponControl.state != ArmRotation.State.Defend)
			{
				MoveHandsToPos(localBasePos);
				yield return null;
			}

			yield return new WaitForSeconds(1f);
		}
	}

	void Update()
	{
		Vector3 basePos = arms.armMovement.handCenterPos;
		localBasePos = mech.transform.InverseTransformPoint(basePos);
	}
}