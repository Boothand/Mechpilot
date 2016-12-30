//using System.Collections;
using UnityEngine;

public class HeadRotation : MechComponent
{
	[Header("Assign References")]
	//Rotation transform for neck, one for each axis
	[SerializeField] Transform aimBaseX;
	[SerializeField] Transform aimBaseY;

	public Vector3 lookDir { get { return aimBaseX.forward; } }

	Vector2 headRotation;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void LimitHeadRotation()
	{
		float angleX = Mathf.DeltaAngle(transform.eulerAngles.y, aimBaseX.eulerAngles.y);
		float angleY = Mathf.DeltaAngle(transform.eulerAngles.x, aimBaseY.eulerAngles.x);
		float xLimit = 70f;
		float yLimit = 87f;

		if (angleX > xLimit)
			headRotation.x -= angleX - xLimit;

		if (angleX < -xLimit)
			headRotation.x -= angleX + xLimit;

		if (angleY > yLimit)
			headRotation.y += angleY - yLimit;

		if (angleY < -yLimit)
			headRotation.y += angleY + yLimit;


		//Set the rotation after correcting headRotation
		aimBaseX.localRotation = Quaternion.Euler(0f, headRotation.x, 0f);
		aimBaseY.localRotation = Quaternion.Euler(-headRotation.y, 0f, 0f);
	}

	public void RunComponent()
	{
		//------------------ LOOKING AROUND ------------------\\

		//Place the head's 'aim base' at the correct position
		aimBaseX.position = hierarchy.head.position;

		//Feed input values into the X and Y rotation of the aim base
		headRotation.x += input.lookHorz * Time.deltaTime * 100f;
		headRotation.y += input.lookVert * Time.deltaTime * 100f;

		//Apply the values to the rotation
		aimBaseX.localRotation = Quaternion.Euler(0f, headRotation.x, 0f);
		aimBaseY.localRotation = Quaternion.Euler(-headRotation.y, 0f, 0f);

		//Correct the aim base's rotation within limits
		LimitHeadRotation();
	}
}