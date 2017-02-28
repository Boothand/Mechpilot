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

	public Vector3 lookDir { get { return forwardDir;/* aimBaseX.forward;*/ } }

	Vector2 headRotation;
	Vector2 targetRotation;

	[SerializeField] Transform chest;
	[SerializeField] float xAngle = 90f;
	[SerializeField] float yAngle = 90f;
	[SerializeField] float speed = 4f;
	Vector3 angle;
	Vector3 forwardDir;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	//void LimitHeadAngles()
	//{
	//	float angleX = Mathf.DeltaAngle(mech.transform.eulerAngles.y, aimBaseX.eulerAngles.y);
	//	float angleY = Mathf.DeltaAngle(mech.transform.eulerAngles.x, aimBaseY.eulerAngles.x);
	//	float xLimit = 70f;
	//	float yLimit = 50f;

	//	if (angleX > xLimit)
	//	{
	//		headRotation.x -= angleX - xLimit;
	//		targetRotation.x = headRotation.x;
	//	}
	//	if (angleX < -xLimit)
	//	{
	//		headRotation.x -= angleX + xLimit;
	//		targetRotation.x = headRotation.x;
	//	}
	//	if (angleY > yLimit)
	//	{
	//		headRotation.y += angleY - yLimit;
	//		targetRotation.y = headRotation.y;
	//	}

	//	if (angleY < -yLimit)
	//	{
	//		headRotation.y += angleY + yLimit;
	//		targetRotation.y = headRotation.y;
	//	}


	//		//Set the rotation after correcting headRotation
	//	aimBaseX.localRotation = Quaternion.Euler(0f, headRotation.x, 0f);
	//	aimBaseY.localRotation = Quaternion.Euler(-headRotation.y, 0f, 0f);
	//}

	public void RunComponent()
	{
		//------------------ LOOKING AROUND ------------------\\

		//Place the head's 'aim base' at the correct position
		//aimBaseX.position = hierarchy.head.position;
		//aimBaseX.forward = forwardDir;

		//float lookHorzInput = Mathf.Clamp(input.lookHorz, -maxTurnSpeed, maxTurnSpeed);
		//float lookVertInput = Mathf.Clamp(input.lookVert, -maxTurnSpeed, maxTurnSpeed);

		////Feed input values into the X and Y rotation of the aim base
		//targetRotation.x += lookHorzInput * Time.deltaTime * turnSpeed;
		//targetRotation.y += lookVertInput * Time.deltaTime * turnSpeed;

		//headRotation.x = Mathf.Lerp(headRotation.x, targetRotation.x, Time.deltaTime * blendSpeed);
		//headRotation.y = Mathf.Lerp(headRotation.y, targetRotation.y, Time.deltaTime * blendSpeed);

		////Apply the values to the rotation
		//aimBaseX.localRotation = Quaternion.Euler(0f, headRotation.x, 0f);
		//aimBaseY.localRotation = Quaternion.Euler(-headRotation.y, 0f, 0f);

		////Correct the aim base's rotation within limits
		//LimitHeadAngles();
	}

	void LateUpdate()
	{
		float xInput = Mathf.Clamp(input.lookHorz, -1f, 1f);
		float yInput = Mathf.Clamp(input.lookVert, -1f, 1f);
		angle.x = Mathf.Lerp(angle.x, xInput * xAngle, Time.deltaTime * speed);
		angle.y = Mathf.Lerp(angle.y, yInput * yAngle, Time.deltaTime * speed);
		Quaternion rotOffset = Quaternion.Euler(angle.y, angle.x, 0f);
		chest.rotation *= rotOffset;

		forwardDir = Quaternion.Euler(0, angle.x, 0) * mech.transform.forward;
		forwardDir.y = 0f;
		forwardDir.Normalize();

		Debug.DrawRay(chest.position, forwardDir, Color.red);
	}
}