//using System.Collections;
using UnityEngine;

public class BodyRotation : MechComponent
{
	//Limits for how far you can rotate the upper body
	[SerializeField] float xAngle = 45f;
	[SerializeField] float yAngle = 45f;

	[SerializeField] float blendSpeed = 6f;
	Vector3 angle;

	[SerializeField] bool tilt;	//Rotates on a different axis!


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	//Apply in LateUpdate so it overrides animation transformations.
	//Tweens the angle to the set limits according to your right stick input.
	//Applies a rotation to the spine.
	protected override void OnLateUpdate()
	{
		float xInput = Mathf.Clamp(input.turnBodyHorz, -1f, 1f);
		float yInput = Mathf.Clamp(input.turnBodyVert, -1f, 1f);

		angle.x = Mathf.Lerp(angle.x, xInput * xAngle, Time.deltaTime * blendSpeed);
		angle.y = Mathf.Lerp(angle.y, yInput * yAngle, Time.deltaTime * blendSpeed);
		Quaternion rotOffset = Quaternion.Euler(-angle.y, angle.x, 0f);

		if (tilt)
		{
			rotOffset = Quaternion.Euler(angle.y, 0f, -angle.x);
		}

		hierarchy.spine.rotation *= rotOffset;
	}
}