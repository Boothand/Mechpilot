using UnityEngine;

public class ArmMovement : MechComponent
{
	[Header("References")]
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;

	[Header("Values")]
	[SerializeField] float armHeight = -0.3f;
	[SerializeField] float armDistance = 0.3f;
	[SerializeField] float maxArmSpeed = 0.85f;
	[SerializeField] float armReach = 1f;

	Vector3 rArmPos, lArmPos;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	Vector3 SetArmPos(Vector3 input, ref Vector3 armPos, Transform shoulder)
	{
		//Limit max input delta
		input = Vector3.ClampMagnitude(input, maxArmSpeed);

		//Transform input direction to local space
		Vector3 localInputDir = mech.transform.TransformDirection(input);

		//Add input values to XY position
		armPos += localInputDir * Time.deltaTime * engineer.energies[ARMS_INDEX];

		//Limit arm's reach on local XY axis
		armPos = Vector3.ClampMagnitude(armPos, armReach);

		//The center of the circular area used for the arm movement
		Vector3 handCentralPos = shoulder.position + Vector3.up * armHeight;

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance;

		//Final position
		return handCentralPos + armPos;
	}

	public void RunComponent()
	{
		Vector3 rInput = new Vector3(input.rArmHorz, input.rArmVert);
		Vector3 lInput = new Vector3(input.lArmHorz, input.lArmVert);

		rHandIKTarget.position = SetArmPos(rInput, ref rArmPos, hierarchy.rShoulder);
		lHandIKTarget.position = SetArmPos(lInput, ref lArmPos, hierarchy.lShoulder);
	}
}