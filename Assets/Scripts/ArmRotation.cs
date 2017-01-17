//using System.Collections;
using UnityEngine;

public class ArmRotation : MechComponent
{
	[SerializeField] float maxRotationSpeed = 200f;
	float targetRot;
	float handRotation;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void RunComponent()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);

		targetRot += rotationInput * Time.deltaTime * maxRotationSpeed * engineer.energies[ARMS_INDEX];

		if (targetRot > 360)
			targetRot -= 360f;

		if (targetRot < -360)
			targetRot += 360f;

		handRotation = Mathf.LerpAngle(handRotation, targetRot, Time.deltaTime * 5f);

		Transform rHandIk = arms.armMovement.rHandIK;

		Vector3 angles = mech.transform.eulerAngles;

		//rHandIk.Rotate(Vector3.up, handRotation);
		//rHandIk.localRotation = Quaternion.Euler(90, rHandIk.localEulerAngles.y, rHandIk.localEulerAngles.z);
		//print(handRotation);

		//float rotationInput = Mathf.Clamp(input.rArmRot, -0.2f, 0.2f);

		//targetRot += rotationInput * Time.deltaTime * maxRotationSpeed * engineer.energies[ARMS_INDEX];

		//targetRot = Mathf.Clamp(targetRot, -1, 1);

		//handRotation = Mathf.Lerp(handRotation, targetRot, Time.deltaTime * 5f);

		//animator.SetFloat("RightHandRotation", handRotation);
	}

	void LateUpdate()
	{
		//hierarchy.rhand.localRotation = Quaternion.Euler(0, -handRotation, 0);

	}
}