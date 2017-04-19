//using System.Collections;
using UnityEngine;

public class SwordRotationOffset : MechComponent
{
	float rotation;
	[SerializeField] float speed = 5f;
	[SerializeField] float maxDegreesX = 20f;
	[SerializeField] float maxDegreesY = 20f;
	float rotX, rotY;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void LateUpdate()
	{
		Transform rIK = arms.getRhandIKTarget;
		float rotInputX = Mathf.Clamp(-input.lArmHorz, -1f, 1f);
		float rotInputY = Mathf.Clamp(input.lArmVert, -1f, 1f);

		rotX = Mathf.Lerp(rotX, rotInputX * maxDegreesX, Time.deltaTime * speed);
		rotY = Mathf.Lerp(rotY, rotInputY * maxDegreesY, Time.deltaTime * speed);

		//blocker.targetRotOffset = Quaternion.Euler(doneRotX, doneRotY, 0f);
		hierarchy.rhand.rotation *= Quaternion.Euler(rotX, rotY, 0f);

#if false
		float speedFactor = speed * Time.deltaTime;
		//X
		if (Mathf.Abs(rotInputX) > 0.1f)
		{
			if (Mathf.Abs(doneRotX) < maxDegreesX * Mathf.Abs(rotInputX))
			{
				doneRotX += rotInputX * speedFactor;
				rIK.rotation *= Quaternion.Euler(rotInputX * speedFactor, 0f, 0f);
			}
		}
		else
		{
			if (doneRotX < -speedFactor)
			{
				doneRotX += speedFactor;
				rIK.rotation *= Quaternion.Euler(speedFactor, 0f, 0f);
			}
			else if (doneRotX > speedFactor)
			{
				doneRotX -= speedFactor;
				rIK.rotation *= Quaternion.Euler(-speedFactor, 0f, 0f);
			}
		}

		doneRotX = Mathf.Clamp(doneRotX, -maxDegreesX, maxDegreesX);
		print(doneRotX);

		//Y
		if (Mathf.Abs(rotInputY) > 0.01f)
		{
			if (Mathf.Abs(doneRotY) < maxDegreesY * Mathf.Abs(rotInputY))
			{
				doneRotY += rotInputY * speedFactor;
				rIK.rotation *= Quaternion.Euler(0f, rotInputY * speedFactor, 0f);
			}
		}
		else
		{
			if (doneRotY < -speedFactor)
			{
				doneRotY += speedFactor;
				rIK.rotation *= Quaternion.Euler(0f, speedFactor, 0f);
			}
			else if (doneRotY > speedFactor)
			{
				doneRotY -= speedFactor;
				rIK.rotation *= Quaternion.Euler(0f, -speedFactor, 0f);
			}
		}
#endif

	}
}