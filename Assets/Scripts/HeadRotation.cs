//using System.Collections;
using UnityEngine;

public class HeadRotation : MechComponent
{
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

	public void RunComponent()
	{

	}

	void LateUpdate()
	{
		float xInput = Mathf.Clamp(input.lookHorz, -1f, 1f);
		float yInput = Mathf.Clamp(input.lookVert, -1f, 1f);
		angle.x = Mathf.Lerp(angle.x, xInput * xAngle, Time.deltaTime * speed);
		angle.y = Mathf.Lerp(angle.y, -yInput * yAngle, Time.deltaTime * speed);
		Quaternion rotOffset = Quaternion.Euler(angle.y, angle.x, 0f);
		chest.rotation *= rotOffset;

		forwardDir = Quaternion.Euler(0, angle.x, 0) * mech.transform.forward;
		forwardDir.y = 0f;
		forwardDir.Normalize();

		Debug.DrawRay(chest.position, forwardDir, Color.red);
	}
}