//using System.Collections;
using UnityEngine;

public class MechRotation : MechComponent
{
	public Vector3 lookDir { get { return forwardDir;/* aimBaseX.forward;*/ } }
	[SerializeField] float rotationSpeed = 40f;
	float rotationAmount;
	
	Vector3 forwardDir;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		Vector3 targetRot = new Vector3(input.lookHorz, input.lookVert);
		rotationAmount = Mathf.Lerp(rotationAmount, targetRot.x, Time.deltaTime * 4f);

		forwardDir = Quaternion.Euler(0, rotationAmount * rotationSpeed, 0) * mech.transform.forward;
		forwardDir.y = 0f;
		forwardDir.Normalize();
	}
}