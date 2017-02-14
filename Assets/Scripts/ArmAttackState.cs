//using System.Collections;
using UnityEngine;

public class ArmAttackState : MechComponent
{
	[Header("Position")]
	[SerializeField] float attackForwardDistance = 0.25f;

	[Header("Rotaton")]
	[SerializeField] float attackRotSpeed = 5f;
	[SerializeField] float retractRotSpeed = 2f;
	[SerializeField] float swingAmount = 120f;
	[SerializeField] float swingAcceleration = 1.4f;
	[SerializeField] float staminaAmount = 15f;

	public float getRotSpeed { get { return attackRotSpeed; } }
	public float getRetractSpeed { get { return retractRotSpeed; } }
	public float getForwardDistance { get { return attackForwardDistance; } }
	public float getSwingAcceleration { get { return swingAcceleration; } }
	public float getStaminaAmount { get { return staminaAmount; } }


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public Quaternion AttackRotation()
	{
		Quaternion rot = Quaternion.Euler(0, swingAmount, 0);
		return arms.armControl.targetWindupRotation * rot;
	}

	public Vector3 AttackPosition()
	{
		Vector3 blockPos = arms.armControl.blockPos;
		Vector3 dir = mech.transform.forward;
		float distance = arms.armAttackState.getForwardDistance * scaleFactor;

		return blockPos + dir * distance;
	}
}