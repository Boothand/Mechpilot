using UnityEngine;

public class ArmMovement : MechComponent
{
	[Header("References")]
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;

	public Transform rHandIK { get { return rHandIKTarget; } }
	public Transform lHandIK { get { return rHandIKTarget; } }

	[Header("Values")]
	[Range(-1f, 1f)]
	[SerializeField] float armHeight = -0.3f;

	[Range(0.05f, 1f)]
	[SerializeField] float armDistance = 0.3f;

	[Range(0.2f, 2f)]
	[SerializeField] float armReach = 1f;

	[Range(0, 20)]
	[SerializeField] float sloppiness = 15f;

	Vector3 rArmPos, lArmPos;
	Vector3 rTargetPos, lTargetPos;
	Vector3 handCentralPos;

	public Vector3 rHandCenterPos
	{
		get
		{
			return hierarchy.rShoulder.position + Vector3.up * armHeight * scaleFactor;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	Vector3 SetArmPos(Vector3 input, ref Vector3 armPos, Transform shoulder)
	{
		//Limit max input delta
		input = Vector3.ClampMagnitude(input, 1f);

		//Transform input direction to local space
		Vector3 worldInputDir = mech.transform.TransformDirection(input);

		//Add input values to XY position
		armPos += worldInputDir * Time.deltaTime * engineer.energies[ARMS_INDEX] * scaleFactor;

		//Limit arm's reach on local XY axis
		armPos = Vector3.ClampMagnitude(armPos, armReach * scaleFactor);

		//The center of the circular area used for the arm movement
		Vector3 handCentralPos = shoulder.position + Vector3.up * armHeight * scaleFactor;

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * scaleFactor;

		//Final position
		return handCentralPos + armPos;
	}

	public void RunComponent()
	{
		Vector3 rInput = new Vector3(input.rArmHorz, input.rArmVert);
		Vector3 lInput = new Vector3(input.lArmHorz, input.lArmVert);

		//NOTE: When falling, hands don't interpolate quickly enough, and will hang behind, may look buggy
		rTargetPos = SetArmPos(rInput, ref rArmPos, hierarchy.rShoulder);
		lTargetPos = SetArmPos(lInput, ref lArmPos, hierarchy.lShoulder);		

		//Interpolate on all axes
		rHandIKTarget.position = Vector3.Lerp(rHandIKTarget.position, rTargetPos, Time.deltaTime * (20f - sloppiness));
		lHandIKTarget.position = Vector3.Lerp(lHandIKTarget.position, lTargetPos, Time.deltaTime * (20f - sloppiness));

		//Lock the position on the local Z axis, so it doesn't interpolate.
		Vector3 rHIKLocal = rHandIKTarget.localPosition;
		Vector3 lHIKLocal = lHandIKTarget.localPosition;

		Vector3 rTargetLocal = mech.transform.InverseTransformPoint(rTargetPos);
		Vector3 lTargetLocal = mech.transform.InverseTransformPoint(lTargetPos);

		rHIKLocal.z = rTargetLocal.z;
		lHIKLocal.z = lTargetLocal.z;
		rHandIKTarget.localPosition = rHIKLocal;
		lHandIKTarget.localPosition = lHIKLocal;
	}
}