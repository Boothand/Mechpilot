using System.Collections;
using UnityEngine;

public class ArmControl : MechComponent
{
	#region Variables/References
	//The IK targets to move and rotate around
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;
	public Transform getRhandIKTarget { get { return rHandIKTarget; } }

	//The different states during combat
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

	//----- Store these so we don't have to calculate them more than once per frame: -----\\

	//The different rotations to interpolate between
	public Quaternion handSideRotation { get; private set; }
	public Quaternion targetWindupRotation { get; private set; }
	public Quaternion targetAttackRotation { get; private set; }

	//The different positions to interpolate between
	public Vector3 blockPos { get; private set; }
	public Vector3 windupPos { get; private set; }
	public Vector3 attackPos { get; private set; }

	[Header("All")]
	[SerializeField] Transform lHandTarget;	//Where to put the left hand (if longsword)
	[SerializeField] float xyPosBlendSpeed = 15f;	//How smoothly to move on the local XY axis
	[SerializeField] float rotationBlendSpeed = 10f;	//How smoothly to blend rotationSpeed
	[SerializeField] float zPosBlendSpeed = 5f;	//How smoothly to move on local Z axis
	[SerializeField] bool invertRotation;

	//The final values to move towards
	Vector3 rTargetPos, lTargetPos;
	Quaternion finalRotation;
	Quaternion toRotation;

	//Value to move from, during linear interpolation
	Quaternion fromRotation;

	//The value from 0 to 1 driving the rotation
	float rotationTimer;

	public Vector3 handCenterPos
	{
		get { return (hierarchy.rShoulder.position + hierarchy.lShoulder.position) / 2 + Vector3.up * arms.armBlockState.getArmHeight * scaleFactor; }
	}

	public delegate void SwordCollision(Collider col);
	public event SwordCollision Clash;
	public event SwordCollision Block;
	public event SwordCollision HitOpponent;
	#endregion


	protected override void OnAwake()
	{
		base.OnAwake();
		state = State.Defend;
		
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

			if (state != State.Defend)
			{
				//Get staggered
				state = State.Staggered;

				StopAllCoroutines();
				StartCoroutine(StaggerRoutine(otherSword, 5f));
			}
		}
	}

	IEnumerator StaggerRoutine(Sword otherSword, float multiplier)
	{
		Vector3 swordVelocity = arms.getWeapon.swordTipVelocity;
		Vector3 otherVelocity = otherSword.swordTipVelocity;
		Vector3 swordTipPos = arms.getWeapon.getSwordTip.position;

		Vector3 newTipPos = swordTipPos + otherVelocity * multiplier;
		//Debug.DrawLine(arms.getWeapon.getSwordTip.position, newTipPos, Color.black);

		//Vector from hand to new sword tip position
		Vector3 newSwordDir = (newTipPos - rHandIKTarget.position).normalized;

		//The world rotation of that vector
		Quaternion newWorldRot = Quaternion.LookRotation(newSwordDir, mech.transform.forward);

		//Transform to localSpace
		Quaternion newLocalRot = Quaternion.Inverse(mech.transform.rotation) * newWorldRot;

		//Debug.DrawLine(arms.getWeapon.getSwordTip.position, newTipPos, Color.black);

		fromRotation = rHandIKTarget.localRotation;
		toRotation = newLocalRot;

		float staggerBeginRotSpeed = 3f;
		float staggerEndRotSpeed = 3f;

		rotationTimer = 0f;
		while (rotationTimer < 1f)
		{
			//Also change the arm's position
			rTargetPos = blockPos + otherVelocity * multiplier;

			//Debug.DrawLine(rHandIKTarget.position, newTipPos, Color.red);
			rotationTimer += Time.deltaTime * staggerBeginRotSpeed;
			yield return null;
		}

		rotationTimer = 0f;

		fromRotation = newLocalRot;
		toRotation = handSideRotation;


		state = State.StaggeredEnd;

		while (rotationTimer < 1f)
		{
			rTargetPos = blockPos + otherVelocity * multiplier * 0.75f;
			rotationTimer += Time.deltaTime * staggerEndRotSpeed;
			toRotation = handSideRotation;

			yield return null;
		}

		rotationTimer = 0;

		state = State.Defend;
	}

	IEnumerator SwingRoutine()
	{
		rotationTimer = 0f;

		//Wait until done winding up
		while (rotationTimer < 1f)
		{
			fromRotation = handSideRotation;
			toRotation = targetWindupRotation;
			rotationTimer += Time.deltaTime * arms.armWindupState.getWindupRotSpeed * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		state = State.WindedUp;
		rotationTimer = 0f;

		//Wait until you're done holding attack
		while (input.attack)
		{
			fromRotation = targetWindupRotation;
			toRotation = targetAttackRotation;

			yield return null;
		}

		fromRotation = targetWindupRotation;
		toRotation = targetAttackRotation;

		//Wait until end of attack
		state = State.Attack;
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * arms.armAttackState.getAttackRotSpeed * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		//The time to wait before retracting
		yield return new WaitForSeconds(0.25f);

		//Retract
		rotationTimer = 0f;
		state = State.AttackRetract;
		while (rotationTimer < 1f)
		{
			fromRotation = targetAttackRotation;
			toRotation = handSideRotation;
			rotationTimer += Time.deltaTime * arms.armAttackState.getAttackRotSpeed * 1.3f * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		state = State.Defend;
	}

	void SetTargetPos()
	{
		switch (state)
		{
			case State.Idle:
				break;

			case State.Defend:
				rTargetPos = blockPos;
				break;

			case State.WindUp:
			case State.WindedUp:
				rTargetPos = windupPos;
				break;

			case State.Attack:
				rTargetPos = attackPos;
				break;

			case State.Staggered:
				//See StaggerRoutine
				break;
		}
	}

	void Update()
	{
		//Store the different target rotations we will use at different times
		handSideRotation = arms.armBlockState.ArmSideRotation();
		targetWindupRotation = arms.armWindupState.WindUpRotation();
		targetAttackRotation = arms.armAttackState.AttackRotation();

		//Store the different target positions we will use at different times
		blockPos = arms.armBlockState.BlockPos();
		windupPos = arms.armWindupState.WindUpPosition();
		attackPos = arms.armAttackState.AttackPosition();

		if (state == State.Defend)
		{
			fromRotation = handSideRotation;
			toRotation = targetWindupRotation;

			if (input.attack)
			{
				state = State.WindUp;
				StartCoroutine(SwingRoutine());
			}
		}

		SetTargetPos();

		float zPosBlendSpeedToUse = zPosBlendSpeed;

		if (state == State.StaggeredEnd)
		{
			zPosBlendSpeedToUse = 0.5f;
		}

		//------------ POSITION ------------\\
		Vector3 localIKPos = rHandIKTarget.localPosition;
		Vector3 localTargetPos = mech.transform.InverseTransformPoint(rTargetPos);
		float xyLerpFactor = Time.deltaTime * xyPosBlendSpeed * energyManager.energies[ARMS_INDEX] * scaleFactor;
		
		//Lerp Z position separately
		localIKPos.x = Mathf.Lerp(localIKPos.x, localTargetPos.x, xyLerpFactor);
		localIKPos.y = Mathf.Lerp(localIKPos.y, localTargetPos.y, xyLerpFactor);
		localIKPos.z = Mathf.Lerp(localIKPos.z, localTargetPos.z, Time.deltaTime * zPosBlendSpeedToUse);

		//Set final position
		rHandIKTarget.localPosition = localIKPos;

		lHandIKTarget.position = lHandTarget.position;


		//------------ ROTATION ------------\\
		Quaternion finalTargetRotation = Quaternion.Lerp(fromRotation, toRotation, rotationTimer);	//Interpolate
		finalRotation = Quaternion.Lerp(finalRotation, finalTargetRotation, Time.deltaTime * rotationBlendSpeed);   //Smooth
		
		//Set final rotation
		rHandIKTarget.localRotation = finalRotation;

		lHandIKTarget.rotation = lHandTarget.rotation;
	}
}