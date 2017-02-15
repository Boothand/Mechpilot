//using System.Collections;
using UnityEngine;

public class ArmBlockState : MechComponent
{
	[Header("Position")]
	[SerializeField] float idleMoveSpeed = 1.25f;
	[Range(-1f, 1f)] [SerializeField] float armHeight = 0.2f;
	public float getArmHeight { get { return armHeight; } }
	[Range(0.05f, 1f)] [SerializeField] float rArmDistance = 0.445f;
	[Range(0.05f, 1f)] [SerializeField] float lArmDistance = 0.232f;
	[Range(0.2f, 2f)] [SerializeField] float armReach = 0.38f;
	bool movingArm;
	public Vector3 rArmPos { get; private set; }
	public Vector3 lArmPos { get; private set; }

	[Header("Rotation")]
	[SerializeField] bool invertRotation;
	public float sideTargetAngle { get; private set; }
	[SerializeField] float rotationSpeed = 250f;
	[SerializeField] float sideRotationLimit = 125f;
	public float getSideRotationLimit { get { return sideRotationLimit; } }
	[SerializeField] Vector3 sideRotOffset;
	
	public delegate void ArmMovement();
	public event ArmMovement OnMoveArmBegin;
	public event ArmMovement OnMoveArm;
	public event ArmMovement OnMoveArmEnd;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public Vector3 BlockPos()
	{
		ArmControl.State state = arms.armControl.state;

		Vector3 inputVec = new Vector3(input.rArmHorz, input.rArmVert);
		//Vector3 lMoveInput = new Vector3(input.lArmHorz, input.lArmVert);   //Only for shield

		Vector3 actualArmPos;

		//Limit max input delta
		inputVec = Vector3.ClampMagnitude(inputVec, 1f);

		float speedToUse = idleMoveSpeed;

		if (state == ArmControl.State.Staggered ||
			state == ArmControl.State.StaggeredEnd)
		{
			speedToUse = idleMoveSpeed / 1.75f;
		}

		//Add input values to XY position
		rArmPos += inputVec * speedToUse * Time.deltaTime * scaleFactor;

		float inputVecMagnitude = inputVec.magnitude;

		#region Event Stuff
		if (!movingArm && inputVecMagnitude > 0.1f)
		{
			movingArm = true;
			if (OnMoveArmBegin != null) { OnMoveArmBegin(); }
		}

		if (movingArm && inputVecMagnitude > 0.1f)
		{
			if (OnMoveArm != null) { OnMoveArm(); }
		}

		if (movingArm && inputVecMagnitude < 0.1f)
		{
			movingArm = false;
			if (OnMoveArmEnd != null) { OnMoveArmEnd(); }
		}
		#endregion

		//Limit arm's reach on local XY axis
		rArmPos = Vector3.ClampMagnitude(rArmPos, armReach * scaleFactor);
		actualArmPos = mech.transform.TransformDirection(rArmPos);

		//The center of the circular area used for the arm movement
		//Vector3 handCentralPos = shoulder.position + Vector3.up * armHeight * scaleFactor;
		Vector3 handCentralPos = arms.armControl.handCenterPos;


		//Dirty check to see which shoulder is used, and what arm distance to use.
		float armDistance = rArmDistance;

		//if (shoulder == hierarchy.lShoulder)
		//{
		//	armDistance = lArmDistance;
		//}

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * scaleFactor;

		//Final position
		return handCentralPos + actualArmPos;
	}

	public Quaternion ArmSideRotation()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);
		ArmControl.State state = arms.armControl.state;
		//Add input values to the target rotation
		int factor = 1;

		if (invertRotation)
			factor = -1;

		if (state != ArmControl.State.Attack &&
			state != ArmControl.State.Staggered)
		{
			sideTargetAngle += factor * rotationInput * Time.deltaTime * rotationSpeed;
		}

		//Wrap
		if (sideTargetAngle > 360)
			sideTargetAngle -= 360f;

		if (sideTargetAngle < -360)
			sideTargetAngle += 360f;

		//Limit rotation angle based on state
		float sideRotLimitToUse = sideRotationLimit;

		switch (arms.armControl.state)
		{
			case ArmControl.State.WindUp:
			case ArmControl.State.WindedUp:
			case ArmControl.State.Attack:
				sideRotLimitToUse = 100f;
				break;

		}

		//Limit target rotation
		sideTargetAngle = Mathf.Clamp(sideTargetAngle, -sideRotLimitToUse, sideRotLimitToUse);

		//Return the rotation
		Quaternion offset = Quaternion.Euler(sideRotOffset);
		Quaternion localRotation = offset * Quaternion.Euler(-sideTargetAngle, 0, 0);
		return localRotation;
	}
}