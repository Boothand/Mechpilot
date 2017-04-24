//using System.Collections;
using UnityEngine;

//This is a pretty useless feature, but allows you to wiggle the sword around in your hand...
public class SwordRotationOffset : MechComponent
{
	[SerializeField] float speed = 5f;	//Multiplier

	//Range
	[SerializeField] float maxDegreesX = 20f;
	[SerializeField] float maxDegreesY = 20f;
	float rotX, rotY;	//The accumulated rotation

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void LateUpdate()
	{
		float rotInputX = Mathf.Clamp(-input.lArmHorz, -1f, 1f);
		float rotInputY = Mathf.Clamp(input.lArmVert, -1f, 1f);

		//Tween rotation on x and y to input * maxdegrees.
		rotX = Mathf.Lerp(rotX, rotInputX * maxDegreesX, Time.deltaTime * speed);
		rotY = Mathf.Lerp(rotY, rotInputY * maxDegreesY, Time.deltaTime * speed);

		//Apply the rotation.
		hierarchy.rhand.rotation *= Quaternion.Euler(rotX, rotY, 0f);
	}
}