using System.Collections;
using UnityEngine;

public class ArmControl : MechComponent
{
	#region Variables/References
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;

	public enum State
	{
		Idle,
		Defend,
		WindUp,
		WindedUp,
		Attack,
		Staggered
	}

	public State state { get; private set; }

	[Header("Idle/Blocking")]
	[SerializeField] float idleMoveSpeed = 1f;
	[SerializeField] Vector3 sideRotOffset;
	[Range(-1f, 1f)] [SerializeField] float armHeight = 0.2f;
	[Range(0.05f, 1f)] [SerializeField] float rArmDistance = 0.445f;
	[Range(0.05f, 1f)] [SerializeField] float lArmDistance = 0.232f;
	[Range(0.2f, 2f)] [SerializeField] float armReach = 0.38f;
	[SerializeField] float rotationSpeed = 200f;
	[SerializeField] float sideRotationLimit = 125f;
	public float sideTargetAngle { get; private set; }
	public float getIdleRotationLimit { get { return sideRotationLimit; } }
	Quaternion handSideRotation;

	[Header("Wind-up")]
	[SerializeField] float windupPullBackDistance = 0.25f;
	[SerializeField] float windupReach = 0.35f;
	[SerializeField] float rotateBackAmount = 24;
	[SerializeField] float windupSpeed = 2f;
	Quaternion targetWindupRotation;

	[Header("Attack")]
	[SerializeField] float attackForwardDistance = 0.25f;
	[SerializeField] float attackSideMovementSpeed = 5f;
	[SerializeField] float attackBlendSpeed = 2f;
	[SerializeField] float attackSpeed = 2f;
	[SerializeField] float swingAmount = 120f;
	Quaternion targetAttackRotation;

	[Header("All")]
	[SerializeField] float baseBlendSpeed = 15f;
	[SerializeField] float blendSpeed = 10f;
	[SerializeField] Transform lHandTarget;
	[SerializeField] bool invertRotation;

	Vector3 rArmPos, lArmPos;
	Vector3 rTargetPos, lTargetPos;
	Quaternion finalRotation;
	Quaternion fromRotation;
	Quaternion toRotation;
	float rotationTimer;

	public Vector3 handCenterPos
	{
		get { return (hierarchy.rShoulder.position + hierarchy.lShoulder.position) / 2 + Vector3.up * armHeight * scaleFactor; }
	}
	#endregion

	protected override void OnAwake()
	{
		base.OnAwake();
		state = State.Defend;

	}


	Vector3 SetArmPos(Vector3 inputVec, ref Vector3 armPos, Transform shoulder)
	{
		Vector3 actualArmPos;
		//Limit max input delta
		inputVec = Vector3.ClampMagnitude(inputVec, 1f);

		//Transform input direction to local space
		//Vector3 worldInputDir = mech.transform.TransformDirection(inputVec);

		float speedToUse = idleMoveSpeed;
		if (state == State.Attack)
		{
			speedToUse = attackSideMovementSpeed;
		}

		//Add input values to XY position
		//armPos += worldInputDir * speedToUse * Time.deltaTime * energyManager.energies[ARMS_INDEX] * scaleFactor;
		armPos += inputVec * speedToUse * Time.deltaTime * energyManager.energies[ARMS_INDEX] * scaleFactor;
		//Limit arm's reach on local XY axis
		armPos = Vector3.ClampMagnitude(armPos, armReach * scaleFactor);
		actualArmPos = mech.transform.TransformDirection(armPos);

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
		//Debug.DrawLine(handCentralPos, handCentralPos + actualArmPos, Color.blue);

		//Final position
		return handCentralPos + actualArmPos;
	}

	Quaternion ArmSideRotation()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);

		//Add input values to the target rotation
		int factor = -1;

		if (invertRotation)
			factor = 1;

		sideTargetAngle += factor * rotationInput * Time.deltaTime * rotationSpeed * energyManager.energies[ARMS_INDEX];

		//Wrap
		if (sideTargetAngle > 360)
			sideTargetAngle -= 360f;

		if (sideTargetAngle < -360)
			sideTargetAngle += 360f;

		//Limit target rotation
		sideTargetAngle = Mathf.Clamp(sideTargetAngle, -sideRotationLimit, sideRotationLimit);

		//Return the rotation
		Quaternion offsetlolz = Quaternion.Euler(sideRotOffset);
		Quaternion localRotation = offsetlolz * Quaternion.Euler(0, -sideTargetAngle, 0);
		return localRotation;
	}

	Quaternion WindUpRotation()
	{
		Quaternion verticalAngle = Quaternion.Euler(-rotateBackAmount, 0, 0);
		return handSideRotation * verticalAngle;
	}

	IEnumerator SwingRoutine()
	{
		rotationTimer = 0f;

		//Winding up
		while (rotationTimer < 1f)
		{
			fromRotation = handSideRotation;
			toRotation = targetWindupRotation;
			rotationTimer += Time.deltaTime * windupSpeed * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		state = State.WindedUp;

		rotationTimer = 0f;

		//Holding wind-up
		while (input.attack)
		{
			fromRotation = targetWindupRotation;
			toRotation = targetAttackRotation;

			yield return null;
		}

		fromRotation = targetWindupRotation;
		toRotation = targetAttackRotation;

		//Releasing attack
		state = State.Attack;
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * attackSpeed * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		yield return new WaitForSeconds(0.25f);

		//Retract

		while (rotationTimer > 0f)
		{
			fromRotation = handSideRotation;
			toRotation = targetAttackRotation;
			rotationTimer -= Time.deltaTime * attackSpeed * 1.3f * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		state = State.Defend;
	}

	float BlendSpeedToUse()
	{
		switch (state)
		{
			case State.WindUp:
				//return 2f;
				break;

			case State.Attack:
				return attackBlendSpeed;
		}

		return baseBlendSpeed;
	}

	void Update()
	{
		//Arm movement inputs
		Vector3 rMoveInput = new Vector3(input.rArmHorz, input.rArmVert);
		Vector3 lMoveInput = new Vector3(input.lArmHorz, input.lArmVert);   //Only for shield

		//Set idle target position
		Vector3 blockPos = SetArmPos(rMoveInput, ref rArmPos, hierarchy.rShoulder);
		rTargetPos = blockPos;
		//lTargetPos = SetArmPos(lMoveInput, ref lArmPos, hierarchy.lShoulder);   //If 1 handed weapon + shield

		//Store the different target rotations we will use at different times
		handSideRotation = ArmSideRotation();
		targetWindupRotation = WindUpRotation();
		targetAttackRotation = targetWindupRotation * Quaternion.Euler(swingAmount, 0, 0);

		float blendSpeedToUse =  BlendSpeedToUse();

		switch (state)
		{
			case State.Idle:

				break;

			case State.Defend:
				fromRotation = handSideRotation;
				toRotation = targetWindupRotation;

				if (input.attack)
				{
					state = State.WindUp;
					StartCoroutine(SwingRoutine());
				}
				break;

			case State.WindUp:
			case State.WindedUp:
				Vector3 targetHandPos = blockPos - mech.transform.forward * windupPullBackDistance * scaleFactor;
				//Vector3 targetCenterPos = handCenterPos;

				//Vector3 dir = targetHandPos - handCenterPos;
				//dir = dir.normalized * windupReach * scaleFactor;
				//rTargetPos = handCenterPos + dir;
				rTargetPos = targetHandPos;
				break;

			case State.Attack:
				
				break;

			case State.Staggered:

				break;
		}

		//Set final position
		float lerpFactor = Time.deltaTime * blendSpeedToUse * energyManager.energies[ARMS_INDEX] * scaleFactor;
		rHandIKTarget.position = Vector3.Lerp(rHandIKTarget.position, rTargetPos, lerpFactor);

		lHandIKTarget.position = lHandTarget.position;
		lHandIKTarget.rotation = lHandTarget.rotation;

		//Set final rotation
		Quaternion finalTargetRotation = Quaternion.Lerp(fromRotation, toRotation, rotationTimer);	//Interpolate
		finalRotation = Quaternion.Lerp(finalRotation, finalTargetRotation, Time.deltaTime * blendSpeed);   //Smooth
		rHandIKTarget.localRotation = finalRotation;
	}
}