//using System.Collections;
using UnityEngine;

public class Test : MechComponent
{
	[SerializeField] Rigidbody chestRb;
	[SerializeField] Transform chest;
	[SerializeField] float xAngle = 90f;
	[SerializeField] float yAngle = 90f;
	[SerializeField] float speed = 4f;
	Vector3 angle;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void LateUpdate()
	{

		//chestRb.AddTorque(Vector3.up * input.lookHorz * force, ForceMode.VelocityChange);
		//chestRb.AddTorque(mech.transform.right * input.lookVert * force, ForceMode.Impulse);
		//chestRb.GetComponent<ConfigurableJoint>().targetRotation *= Quaternion.Euler(input.lookHorz * force, 0f, 0f);
		float xInput = Mathf.Clamp(input.lookHorz, -1f, 1f);
		float yInput = Mathf.Clamp(input.lookVert, -1f, 1f);
		angle.x = Mathf.Lerp(angle.x, xInput * xAngle, Time.deltaTime * speed);
		angle.y = Mathf.Lerp(angle.y, yInput * yAngle, Time.deltaTime * speed);
		chest.transform.rotation *= Quaternion.Euler(0f, angle.x, 0f);
		chest.transform.rotation *= Quaternion.Euler(angle.y, 0f, 0f);
	}
}