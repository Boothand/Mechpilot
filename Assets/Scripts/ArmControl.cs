using System.Collections;
using UnityEngine;

public class ArmControl : MechComponent
{
	#region Variables/References
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;

	public Transform getRhandIKTarget { get { return rHandIKTarget; } }

	public enum State
	{
		Idle,
		Defend,
		WindUp,
		WindedUp,
		Attack,
		AttackRetract,
		Staggered,
		StaggeredEnd
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
	[SerializeField] float xyPosBlendSpeed = 15f;
	[SerializeField] float rotationBlendSpeed = 10f;
	[SerializeField] float zPosBlendSpeed = 5f;
	[SerializeField] Transform lHandTarget;
	[SerializeField] bool invertRotation;

	Vector3 rArmPos, lArmPos;
	Vector3 rTargetPos, lTargetPos;
	Quaternion finalRotation;
	Quaternion fromRotation;
	Quaternion toRotation;
	float rotationTimer;
	bool movingArm;

	public Vector3 handCenterPos
	{
		get { return (hierarchy.rShoulder.position + hierarchy.lShoulder.position) / 2 + Vector3.up * armHeight * scaleFactor; }
	}

	public delegate void SwordCollision(Collider col);
	public event SwordCollision Clash;
	public event SwordCollision Block;
	public event SwordCollision HitOpponent;
	public delegate void ArmMovement();
	public event ArmMovement OnMoveArmBegin;
	public event ArmMovement OnMoveArm;
	public event ArmMovement OnMoveArmEnd;
	#endregion

	[SerializeField] Transform test;
	[SerializeField] Vector3 offsetthing;
	protected override void OnAwake()
	{
		base.OnAwake();
		state = State.Defend;

		//test = GameObject.Find("test").transform;
		arms.getWeapon.OnCollision -= SwordCollide;
		arms.getWeapon.OnCollision += SwordCollide;
	}

	void SwordCollide(Collision col)
	{
		Sword mySword = arms.getWeapon;
		Sword otherSword = col.transform.GetComponent<Sword>();
		

		if (otherSword)
		{
			float impact = otherSword.swordTipVelocity.magnitude + mySword.swordTipVelocity.magnitude;
			impact *= 15f;
			impact = Mathf.Clamp01(impact);

			//Play sound
			if (!otherSword.playingSwordSound)
			{
				mySword.PlayClashSound(impact);
			}


			//Get staggered
			state = State.Staggered;

			StopAllCoroutines();
			StartCoroutine(StaggerRoutine(otherSword, 1f));
		}
	}

	IEnumerator StaggerRoutine(Sword otherSword, float multiplier)
	{
		Vector3 swordVelocity = arms.getWeapon.swordTipVelocity;
		Vector3 otherVelocity = otherSword.swordTipVelocity * multiplier;

		Vector3 newTipPos = arms.getWeapon.getSwordTip.position + otherVelocity * 5f;
		Debug.DrawLine(arms.getWeapon.getSwordTip.position, newTipPos, Color.black);

		Vector3 newSwordDir = (newTipPos - rHandIKTarget.position).normalized;
		Quaternion newWorldRot = Quaternion.LookRotation(newSwordDir, mech.transform.forward);

		//Transform to localSpace
		Quaternion newLocalRot = Quaternion.Inverse(mech.transform.rotation) * newWorldRot;

		Debug.DrawLine(arms.getWeapon.getSwordTip.position, newTipPos, Color.black);
		//print(otherVelocity.magnitude.ToString("0.000"));
		fromRotation = rHandIKTarget.localRotation;
		toRotation = newLocalRot;

		rotationTimer = 0f;
		while (rotationTimer < 1f)
		{
			Debug.DrawLine(rHandIKTarget.position, newTipPos, Color.red);
			rotationTimer += Time.deltaTime * 4f;
			yield return null;
		}

		rotationTimer = 0f;

		fromRotation = newLocalRot;
		toRotation = handSideRotation;
		state = State.StaggeredEnd;

		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * 2f;
			toRotation = handSideRotation;

			yield return null;
		}

		rotationTimer = 0;

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

		//Add input values to XY position
		armPos += inputVec * speedToUse * Time.deltaTime * energyManager.energies[ARMS_INDEX] * scaleFactor;

		float inputVecMagnitude = inputVec.magnitude;

		#region Event Stuff
		if (!movingArm && inputVecMagnitude > 0.1f)
		{
			movingArm = true;

			if (OnMoveArmBegin != null)
				OnMoveArmBegin();
		}

		if (movingArm && inputVecMagnitude > 0.1f)
		{
			if (OnMoveArm != null)
				OnMoveArm();
		}

		if (movingArm && inputVecMagnitude < 0.1f)
		{
			movingArm = false;

			if (OnMoveArmEnd != null)
				OnMoveArmEnd();
		}
		#endregion

		//Limit arm's reach on local XY axis
		armPos = Vector3.ClampMagnitude(armPos, armReach * scaleFactor);
		actualArmPos = mech.transform.TransformDirection(armPos);

		//The center of the circular area used for the arm movement
		//Vector3 handCentralPos = shoulder.position + Vector3.up * armHeight * scaleFactor;
		Vector3 handCentralPos = handCenterPos;


		//Dirty check to see which shoulder is used, and what arm distance to use.
		float armDistance = rArmDistance;

		if (shoulder == hierarchy.lShoulder)
		{
			armDistance = lArmDistance;
		}

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * scaleFactor;

		//Final position
		return handCentralPos + actualArmPos;
	}

	Quaternion ArmSideRotation()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);

		//Add input values to the target rotation
		int factor = 1;

		if (invertRotation)
			factor = -1;

		if (state != State.Attack &&
			state != State.Staggered)
		{
			sideTargetAngle += factor * rotationInput * Time.deltaTime * rotationSpeed * energyManager.energies[ARMS_INDEX];
		}

		//Wrap
		if (sideTargetAngle > 360)
			sideTargetAngle -= 360f;

		if (sideTargetAngle < -360)
			sideTargetAngle += 360f;

		//Limit target rotation
		sideTargetAngle = Mathf.Clamp(sideTargetAngle, -sideRotationLimit, sideRotationLimit);

		//Return the rotation
		Quaternion offset = Quaternion.Euler(sideRotOffset);
		Quaternion localRotation = offset * Quaternion.Euler(0, -sideTargetAngle, 0);
		return localRotation;
	}

	Quaternion WindUpRotation()
	{
		Quaternion verticalAngle = Quaternion.Euler(rotateBackAmount, 0, 0);
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
		state = State.AttackRetract;
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

		return xyPosBlendSpeed;
	}

	void Update()
	{
		//Vector3 dir = (test.position - rHandIKTarget.position).normalized;

		//Quaternion worldRot = Quaternion.LookRotation(dir, mech.transform.forward);
		////worldRot.eulerAngles = new Vector3(worldRot.eulerAngles.x, worldRot.eulerAngles.y, worldRot.eulerAngles.z);
		//Quaternion localRot = Quaternion.Inverse(mech.transform.rotation) * worldRot;
		//rHandIKTarget.localRotation = localRot;

		//return;

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
		targetAttackRotation = targetWindupRotation * Quaternion.Euler(-swingAmount, 0, 0);

		//float blendSpeedToUse =  BlendSpeedToUse();

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
				rTargetPos = blockPos + mech.transform.forward * attackForwardDistance * scaleFactor;
				break;

			case State.Staggered:

				break;
		}

		//Set final position
		Vector3 localIKPos = rHandIKTarget.localPosition;
		Vector3 localTargetPos = mech.transform.InverseTransformPoint(rTargetPos);
		float xyLerpFactor = Time.deltaTime * xyPosBlendSpeed * energyManager.energies[ARMS_INDEX] * scaleFactor;
		//Lerp Z position separately
		localIKPos.x = Mathf.Lerp(localIKPos.x, localTargetPos.x, xyLerpFactor);
		localIKPos.y = Mathf.Lerp(localIKPos.y, localTargetPos.y, xyLerpFactor);
		localIKPos.z = Mathf.Lerp(localIKPos.z, localTargetPos.z, Time.deltaTime * zPosBlendSpeed);

		rHandIKTarget.localPosition = localIKPos;

		lHandIKTarget.position = lHandTarget.position;
		lHandIKTarget.rotation = lHandTarget.rotation;

		//Set final rotation
		Quaternion finalTargetRotation = Quaternion.Lerp(fromRotation, toRotation, rotationTimer);	//Interpolate
		finalRotation = Quaternion.Lerp(finalRotation, finalTargetRotation, Time.deltaTime * rotationBlendSpeed);   //Smooth
		rHandIKTarget.localRotation = finalRotation;
	}
}