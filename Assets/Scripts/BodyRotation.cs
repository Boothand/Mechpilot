//using System.Collections;
using UnityEngine;

public class BodyRotation : MechComponent
{
	[SerializeField] Transform chest;
	[SerializeField] float xAngle = 90f;
	[SerializeField] float yAngle = 90f;
	[SerializeField] float blendSpeed = 6f;
	[SerializeField] float rotationSpeed = 0.05f;
	Vector3 angle;

	float inputXPersist, inputYPersist;

	[SerializeField] bool tilt;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void LateUpdate()
	{
		float xInput = Mathf.Clamp(input.turnBodyHorz, -1f, 1f);
		float yInput = Mathf.Clamp(input.turnBodyVert, -1f, 1f);

		//inputXPersist += xInput * rotationSpeed;
		//inputYPersist += yInput * rotationSpeed;

		//inputXPersist = Mathf.Clamp(inputXPersist, -1f, 1f);
		//inputYPersist = Mathf.Clamp(inputYPersist, -1f, 1f);

		angle.x = Mathf.Lerp(angle.x, xInput * xAngle, Time.deltaTime * blendSpeed);
		angle.y = Mathf.Lerp(angle.y, yInput * yAngle, Time.deltaTime * blendSpeed);
		Quaternion rotOffset = Quaternion.Euler(-angle.y, angle.x, 0f);

		if (tilt)
		{
			rotOffset = Quaternion.Euler(angle.y, 0f, -angle.x);
		}

		chest.rotation *= rotOffset;
	}
}