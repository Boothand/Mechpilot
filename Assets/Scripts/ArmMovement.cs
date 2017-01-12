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
	Vector3 rTargetPos, lTargetPos;

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
		armPos += localInputDir * Time.deltaTime * engineer.energies[ARMS_INDEX] * transform.root.localScale.y;

		//Limit arm's reach on local XY axis
		armPos = Vector3.ClampMagnitude(armPos, armReach * transform.root.localScale.y);

		//The center of the circular area used for the arm movement
		Vector3 handCentralPos = shoulder.position + Vector3.up * armHeight * transform.root.localScale.y;

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * transform.root.localScale.y;

		//Final position
		return handCentralPos + armPos;
	}

	public void RunComponent()
	{
		Vector3 rInput = new Vector3(input.rArmHorz, input.rArmVert);
		Vector3 lInput = new Vector3(input.lArmHorz, input.lArmVert);

		//NOTE: When falling, hands don't interpolate quickly enough, and will hang behind
		rTargetPos = SetArmPos(rInput, ref rArmPos, hierarchy.rShoulder);
		lTargetPos = SetArmPos(lInput, ref lArmPos, hierarchy.lShoulder);
		rHandIKTarget.position = Vector3.Lerp(rHandIKTarget.position, rTargetPos, Time.deltaTime * 5f);
		lHandIKTarget.position = Vector3.Lerp(lHandIKTarget.position, lTargetPos, Time.deltaTime * 5f);
	}
}