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
	[SerializeField] float rArmDistance = 0.3f;

	[Range(0.05f, 1f)]
	[SerializeField] float lArmDistance = 0.3f;

	[Range(0.2f, 2f)]
	[SerializeField] float armReach = 1f;

	[Range(0, 20)]
	[SerializeField] float sloppiness = 15f;

	[SerializeField] float tiss = 7f;

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

		//Dirty check to see which shoulder is used, and what arm distance to use.
		float armDistance = rArmDistance;

		if (shoulder == hierarchy.lShoulder)
		{
			armDistance = lArmDistance;
		}

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * scaleFactor;

		//Final position
		return handCentralPos + armPos;
	}

	public void RunComponent()
	{
		Vector3 rInput = new Vector3(input.rArmHorz, input.rArmVert);
		Vector3 lInput = new Vector3(input.lArmHorz, input.lArmVert);
		
		rTargetPos = SetArmPos(rInput, ref rArmPos, hierarchy.rShoulder);
		lTargetPos = SetArmPos(lInput, ref lArmPos, hierarchy.lShoulder);

		float blendTime = 20f - sloppiness;
		switch (arms.weaponControl.state)
		{
			case WeaponControl.State.Defend:
				rTargetPos.z = mech.transform.TransformPoint(Vector3.one * rArmDistance).z;
				break;

			case WeaponControl.State.WindUp:

				Vector3 normalizedPos = (rTargetPos - rHandCenterPos);
				rTargetPos.z = mech.transform.TransformPoint(Vector3.one * 0.2f).z;
				normalizedPos.z = rTargetPos.z;
				normalizedPos.Normalize();

				Vector3 dir = rTargetPos - rHandCenterPos;
				dir.z = 0;
				dir = dir.normalized * tiss;
				Vector3 armPos = rHandCenterPos + dir;

				Debug.DrawRay(rHandCenterPos, dir, Color.red);
				rTargetPos.x = armPos.x;
				rTargetPos.y = armPos.y;
				//rTargetPos.x = normalizedPos.x;
				//rTargetPos.y = normalizedPos.y;
				break;

			case WeaponControl.State.Attack:
				rTargetPos.z = mech.transform.TransformPoint(Vector3.one * 1f).z;
				//rTargetPos = rHandCenterPos + mech.transform.forward * 18f;
				blendTime = 2f;
				break;
		}

		//Interpolate on all axes
		rHandIKTarget.position = Vector3.Lerp(rHandIKTarget.position, rTargetPos, Time.deltaTime * blendTime);
		lHandIKTarget.position = Vector3.Lerp(lHandIKTarget.position, lTargetPos, Time.deltaTime * blendTime);
	}
}