//using System.Collections;
using UnityEngine;

public class ArmRotation : MechComponent
{
	[SerializeField] Transform ikTarget;
	[SerializeField] float maxRotationSpeed = 200f;
	float targetRot;
	float handRotation;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void RunComponent()
	{
		float rotationInput = input.rArmRot;
		rotationInput = Mathf.Clamp(rotationInput, -1f, 1f);

		targetRot += rotationInput * Time.deltaTime * maxRotationSpeed * engineer.energies[ARMS_INDEX];

		if (targetRot > 360)
			targetRot -= 360f;

		if (targetRot < -360)
			targetRot += 360f;

		handRotation = Mathf.LerpAngle(handRotation, targetRot, Time.deltaTime * 5f);
	}

	void LateUpdate()
	{
		hierarchy.rhand.localRotation = Quaternion.Euler(0, -handRotation, 0);
	}
}