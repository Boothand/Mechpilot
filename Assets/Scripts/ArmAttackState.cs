//using System.Collections;
using UnityEngine;

public class ArmAttackState : MechComponent
{
	[Header("Position")]
	[SerializeField] float attackForwardDistance = 0.25f;

	[Header("Rotaton")]
	[SerializeField] float attackRotSpeed = 5f;
	[SerializeField] float swingAmount = 120f;

	public float getAttackRotSpeed { get { return attackRotSpeed; } }
	public float getAttackForwardDistance { get { return attackForwardDistance; } }


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public Quaternion AttackRotation()
	{
		Quaternion rot = Quaternion.Euler(-swingAmount, 0, 0);
		return arms.armControl.targetWindupRotation * rot;
	}

	public Vector3 AttackPosition()
	{
		Vector3 blockPos = arms.armControl.blockPos;
		Vector3 dir = mech.transform.forward;
		float distance = arms.armAttackState.getAttackForwardDistance * scaleFactor;

		return blockPos + dir * distance;
	}
}