//using System.Collections;
using UnityEngine;

public class ArmWindupState : MechComponent
{
	[Header("Position")]
	[SerializeField] float windupPullBackDistance = 0.19f;

	[Header("Rotation")]
	[SerializeField] float rotateBackAmount = 24;
	[SerializeField] float windupRotSpeed = 2.5f;

	public float getWindupRotSpeed { get { return windupRotSpeed; } }
	public float getWindupPullBackDistance { get { return windupPullBackDistance; } }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public Quaternion WindUpRotation()
	{
		Quaternion verticalAngle = Quaternion.Euler(0, -rotateBackAmount, 0);
		return arms.armControl.handSideRotation * verticalAngle;
	}

	public Vector3 WindUpPosition()
	{
		Vector3 blockPos = arms.armControl.blockPos;
		Vector3 dir = -mech.transform.forward;
		float length = arms.armWindupState.getWindupPullBackDistance * scaleFactor;

		return blockPos + dir * length;
	}
}