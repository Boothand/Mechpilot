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
		//float rotationInput = input.rArmRot;
		//rotationInput = Mathf.Clamp(rotationInput, -1f, 1f);

		//targetRot += rotationInput * Time.deltaTime * maxRotationSpeed * engineer.energies[ARMS_INDEX];

		//if (targetRot > 360)
		//	targetRot -= 360f;

		//if (targetRot < -360)
		//	targetRot += 360f;

		//handRotation = Mathf.LerpAngle(handRotation, targetRot, Time.deltaTime * 5f);
		
		float rotationInput = Mathf.Clamp(input.rArmRot, -0.2f, 0.2f);

		targetRot += rotationInput * Time.deltaTime * maxRotationSpeed * engineer.energies[ARMS_INDEX];

		targetRot = Mathf.Clamp(targetRot, -1, 1);

		handRotation = Mathf.Lerp(handRotation, targetRot, Time.deltaTime * 5f);

		animator.SetFloat("RightHandRotation", handRotation);
	}

	void LateUpdate()
	{
		//hierarchy.rhand.localRotation = Quaternion.Euler(0, -handRotation, 0);

	}
}