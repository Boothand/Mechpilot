//using System.Collections;
using UnityEngine;

public class HeadRotation : MechComponent
{
	[Header("Assign References")]
	//Rotation transform for neck, one for each axis
	[SerializeField] Transform aimBaseX;
	[SerializeField] Transform aimBaseY;

	[SerializeField] float turnSpeed = 100f;
	[SerializeField] float blendSpeed = 5f;

	float maxTurnSpeed = 1f;

	public Vector3 lookDir { get { return aimBaseX.forward; } }

	Vector2 headRotation;
	Vector2 targetRotation;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void LimitHeadAngles()
	{
		float angleX = Mathf.DeltaAngle(mech.transform.eulerAngles.y, aimBaseX.eulerAngles.y);
		float angleY = Mathf.DeltaAngle(mech.transform.eulerAngles.x, aimBaseY.eulerAngles.x);
		float xLimit = 70f;
		float yLimit = 50f;

		if (angleX > xLimit)
		{
			headRotation.x -= angleX - xLimit;
			targetRotation.x = headRotation.x;
		}
		if (angleX < -xLimit)
		{
			headRotation.x -= angleX + xLimit;
			targetRotation.x = headRotation.x;
		}
		if (angleY > yLimit)
		{
			headRotation.y += angleY - yLimit;
			targetRotation.y = headRotation.y;
		}

		if (angleY < -yLimit)
		{
			headRotation.y += angleY + yLimit;
			targetRotation.y = headRotation.y;
		}


			//Set the rotation after correcting headRotation
		aimBaseX.localRotation = Quaternion.Euler(0f, headRotation.x, 0f);
		aimBaseY.localRotation = Quaternion.Euler(-headRotation.y, 0f, 0f);
	}

	public void RunComponent()
	{
		//------------------ LOOKING AROUND ------------------\\

		//Place the head's 'aim base' at the correct position
		aimBaseX.position = hierarchy.head.position;

		float lookHorzInput = Mathf.Clamp(input.lookHorz, -maxTurnSpeed, maxTurnSpeed);
		float lookVertInput = Mathf.Clamp(input.lookVert, -maxTurnSpeed, maxTurnSpeed);

		//Feed input values into the X and Y rotation of the aim base
		targetRotation.x += lookHorzInput * energyManager.energies[PILOT_INDEX] * Time.deltaTime * turnSpeed;
		targetRotation.y += lookVertInput * energyManager.energies[PILOT_INDEX] * Time.deltaTime * turnSpeed;

		headRotation.x = Mathf.Lerp(headRotation.x, targetRotation.x, Time.deltaTime * blendSpeed);
		headRotation.y = Mathf.Lerp(headRotation.y, targetRotation.y, Time.deltaTime * blendSpeed);

		//Apply the values to the rotation
		aimBaseX.localRotation = Quaternion.Euler(0f, headRotation.x, 0f);
		aimBaseY.localRotation = Quaternion.Euler(-headRotation.y, 0f, 0f);

		//Correct the aim base's rotation within limits
		LimitHeadAngles();
	}
}