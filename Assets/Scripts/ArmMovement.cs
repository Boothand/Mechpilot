using UnityEngine;

public class ArmMovement : MechComponent
{
	[Header("References")]
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;

	public Transform rHandIK { get { return rHandIKTarget; } }
	public Transform lHandIK { get { return rHandIKTarget; } }

	[Header("Idle/Blocking")]
	[SerializeField] float idleMoveSpeed = 1f;
	[Range(-1f, 1f)] [SerializeField] float armHeight = -0.3f;
	[Range(0.05f, 1f)] [SerializeField] float rArmDistance = 0.3f;
	[Range(0.05f, 1f)] [SerializeField] float lArmDistance = 0.3f;
	[Range(0.2f, 2f)] [SerializeField] float armReach = 1f;


	[Header("Wind-up")]
	[SerializeField] float windupPullBackDistance = 7.8f;
	[SerializeField] float windupReach = 7f;

	[Header("Attack")]
	[SerializeField] float attackForwardDistance = 5f;
	[SerializeField] float attackSideMovementSpeed = 5f;
	[SerializeField] float attackBlendTime = 2f;

	[Header("All")]
	[SerializeField] float baseBlendSpeed = 5f;

	Vector3 rArmPos, lArmPos;
	Vector3 rTargetPos, lTargetPos;
	Vector3 handCentralPos;

	public Vector3 rHandCenterPos
	{
		get { return hierarchy.rShoulder.position + Vector3.up * armHeight * scaleFactor; }
	}

	public Vector3 handCenterPos
	{
		get { return (hierarchy.rShoulder.position + hierarchy.lShoulder.position) / 2 + Vector3.up * armHeight * scaleFactor; }
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

		float speedToUse = idleMoveSpeed;
		if (arms.armRotation.state == ArmRotation.State.Attack)
		{
			speedToUse = attackSideMovementSpeed;
		}

		//Add input values to XY position
		armPos += worldInputDir * speedToUse * Time.deltaTime * energyManager.energies[ARMS_INDEX] * scaleFactor;

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

		Debug.DrawRay(shoulder.position, Vector3.up, Color.blue);

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * scaleFactor;

		//Final position
		return handCentralPos + armPos;
	}

	void Update()
	{
		Vector3 rInput = new Vector3(input.rArmHorz, input.rArmVert);
		Vector3 lInput = new Vector3(input.lArmHorz, input.lArmVert);
		
		rTargetPos = SetArmPos(rInput, ref rArmPos, hierarchy.rShoulder);
		lTargetPos = SetArmPos(lInput, ref lArmPos, hierarchy.lShoulder);

		float blendSpeedToUse = baseBlendSpeed;

		switch (arms.armRotation.state)
		{
			case ArmRotation.State.Defend:
				//rTargetPos += mech.transform.TransformPoint(new Vector3(0, 0, rArmDistance));
				break;

			case ArmRotation.State.WindUp:
			case ArmRotation.State.WindedUp:

				Vector3 targetHandPos = rTargetPos - mech.transform.forward * windupPullBackDistance * scaleFactor;
				Vector3 targetCenterPos = rHandCenterPos;

				Vector3 dir = targetHandPos - rHandCenterPos;
				dir = dir.normalized * windupReach * scaleFactor;
				rTargetPos = rHandCenterPos + dir;

				
				//if (rHandIKTarget.position.y < handCenterPos.y)
				//{
				//	print(rHandIK.localPosition);
				//	Vector3 localPos = rHandIKTarget.localPosition;
				//	if (localPos.x < 0.3f && localPos.x > 0f)
				//	{
				//		localPos.x = 0.3f;
				//	}
				//	if (localPos.x > -0.3f && localPos.x < 0f)
				//	{
				//		localPos.x = -0.3f;
				//	}

				//	localPos.y = 1.8f;

				//	rHandIKTarget.localPosition = localPos;
				//}

				break;

			case ArmRotation.State.Attack:
				rTargetPos += mech.transform.forward * attackForwardDistance * scaleFactor;

				blendSpeedToUse = attackBlendTime;
				break;
		}

		//Interpolate on all axes
		rHandIKTarget.position = Vector3.Lerp(rHandIKTarget.position, rTargetPos, Time.deltaTime * blendSpeedToUse * energyManager.energies[ARMS_INDEX] * scaleFactor);
		
		//Shield
		//lHandIKTarget.position = Vector3.Lerp(lHandIKTarget.position, lTargetPos, Time.deltaTime * blendTime);

		//Two-handed sword solution:
		lHandIKTarget.position = rHandIKTarget.position;
	}
}