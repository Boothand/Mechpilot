using System.Collections;
using UnityEngine;

public class ArmControl : MechComponent
{
	#region Variables/References
	//The IK targets to move and rotate around
	[Header("References")]
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] public Transform lHandIKTarget;
	[SerializeField] Transform lHandTarget;	//Where to put the left hand (if longsword)
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
		StaggeredEnd,
		BlockStaggered,
		BlockStaggeredEnd,
		GetHitStaggered
	}
	public State state { get; private set; }
	public State prevState { get; private set; }

	//----- Store these so we don't have to calculate them more than once per frame: -----\\

	//The different rotations to interpolate between
	public Quaternion handSideRotation { get; private set; }
	public Quaternion targetWindupRotation { get; private set; }
	public Quaternion targetAttackRotation { get; private set; }

	//The different positions to interpolate between
	public Vector3 blockPos { get; private set; }
	public Vector3 windupPos { get; private set; }
	public Vector3 attackPos { get; private set; }

	[Header("Smoothing")]
	[SerializeField] float xyPosBlendSpeed = 15f;	//How smoothly to move on the local XY axis
	[SerializeField] float rotationBlendSpeed = 10f;	//How smoothly to blend rotationSpeed
	[SerializeField] float zPosBlendSpeed = 5f;	//How smoothly to move on local Z axis

	[Header("Other")]
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
	}

	void Start()
	{
		//arms.getWeapon.OnCollision -= SwordCollide;
		//arms.getWeapon.OnCollision += SwordCollide;
		healthManager.OnGetHit -= OnGetHit;
		healthManager.OnGetHit += OnGetHit;
	}

	void OnGetHit()
	{
		StopAllCoroutines();
		StartCoroutine(GetHitStaggerRoutine());
	}

	void SwordCollide(Collider col)
	{
		Sword mySword = arms.getWeapon;
		Collidable other = col.transform.GetComponent<Collidable>();
		float myImpact = mySword.swordTipVelocity.magnitude / 300f;
		ArmStaggerState stagg = arms.armStaggerState;

		if (other)
		{
			if (other is Sword)
			{
				#region Sword collides with sword

				Sword otherSword = other as Sword;
				State otherPrevState = otherSword.arms.armControl.prevState;
				float theirImpact = otherSword.swordTipVelocity.magnitude / 300f;

				float impact = theirImpact + myImpact;
				impact = Mathf.Clamp01(impact);

				//Play sound, make sure not both swords play it
				if (!otherSword.playingSwordSound)
				{
					mySword.PlayClashSound(impact);
				}

				//STAGGERING
				//When attacking:
				if (state == State.Attack)
				{
					//If they block my attack
					if (otherPrevState == State.Defend ||
						otherPrevState == State.WindUp ||
						otherPrevState == State.WindedUp ||
						otherPrevState == State.AttackRetract)
					{
						StopAllCoroutines();
						StartCoroutine(StaggerRoutine(otherSword, stagg.getBlockedMultiplier, stagg.getStaggerEndRotSpeed));
					}

					//If both swords attack and clash
					if (otherPrevState == State.Attack)
					{
						StopAllCoroutines();
						StartCoroutine(StaggerRoutine(otherSword, stagg.getClashMultiplier, stagg.getStaggerEndRotSpeed));
					}
				}

				if (otherPrevState == State.Attack)
				{
					//Staggering when I'm defending:
					if (state == State.Defend ||
						state == State.WindUp ||
						state == State.AttackRetract)
					{
						StopAllCoroutines();
						StartCoroutine(StaggerRoutine(otherSword, stagg.getBlockMultiplier, stagg.getBlockStaggerEndRotSpeed));
					}

					if (state == State.WindedUp)
					{
						StopAllCoroutines();
						StartCoroutine(StaggerRoutine(otherSword, stagg.getBlockMultiplier, stagg.getBlockStaggerEndRotSpeed / 2, true));
					}
				}
				#endregion
			}
			else
			{
				//If I hit anything else than a sword
				if (prevState == State.Attack)
				{
					StopAllCoroutines();
					StartCoroutine(StaggerRoutine(null, myImpact / 30f, stagg.getStaggerEndRotSpeed));

					if (other is BodyPart)
					{
						
					}
				}
			}
		}
	}

	IEnumerator GetHitStaggerRoutine()
	{
		state = State.GetHitStaggered;

		fromRotation = rHandIKTarget.localRotation;

		rotationTimer = 0f;
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime;
			rTargetPos = blockPos;
			toRotation = handSideRotation;
			yield return null;
		}
		//yield return new WaitForSeconds(1f);

		rotationTimer = 0f;
		state = State.Defend;
		yield return null;
	}

	IEnumerator StaggerRoutine(Sword otherSword, float multiplier, float speed, bool gotoWindedUp = false)
	{
		state = State.Staggered;
		Vector3 otherVelocity = Vector3.zero;

		if (otherSword)
		{
			otherVelocity = otherSword.swordTipVelocity;
		}
		else
		{
			otherVelocity = -arms.getWeapon.swordTipVelocity;
		}

		Quaternion newLocalSwordRot = arms.armStaggerState.StaggerRotation(otherVelocity, multiplier);		

		fromRotation = rHandIKTarget.localRotation;
		toRotation = newLocalSwordRot;		

		rotationTimer = 0f;
		while (rotationTimer < 1f)
		{
			//Also change the arm's position
			rTargetPos = blockPos + otherVelocity * multiplier; //arms.armStaggerState.StaggerPosition(otherSword, multiplier);
			
			rotationTimer += Time.deltaTime * speed;
			yield return null;
		}

		rotationTimer = 0f;

		fromRotation = newLocalSwordRot;
		toRotation = handSideRotation;

		state = State.StaggeredEnd;

		while (rotationTimer < 1f)
		{
			rTargetPos = blockPos + otherVelocity * multiplier * 0.75f;// = arms.armStaggerState.StaggerPosition(otherSword, multiplier * 0.75f);

			rotationTimer += Time.deltaTime * speed;
			toRotation = handSideRotation;

			if (gotoWindedUp)
			{
				toRotation = targetWindupRotation;
				rTargetPos = windupPos;
			}
			yield return null;
		}

		rotationTimer = 0;
		
		state = State.Defend;

		if (gotoWindedUp)
			state = State.WindedUp;
	}

	IEnumerator WindupRoutine()
	{
		rotationTimer = 0f;

		//Wait until done winding up
		while (rotationTimer < 1f)
		{
			fromRotation = handSideRotation;
			toRotation = targetWindupRotation;
			rotationTimer += Time.deltaTime * arms.armWindupState.getWindupRotSpeed;
			yield return null;
		}

		state = State.WindedUp;
	}

	IEnumerator SwingRoutine()
	{
		rotationTimer = 0f;
		energyManager.SpendStamina(arms.armAttackState.getStaminaAmount);
		bool keepStretchedOut = true;

		float maxRotationTime = 1.8f;
		
		while (rotationTimer < maxRotationTime && keepStretchedOut)
		{
			Vector3 inputVec = new Vector3(input.rArmHorz, input.rArmVert);
			rotationTimer += /*(rotationTimer * arms.armAttackState.getSwingAcceleration) +*/ Time.deltaTime * arms.armAttackState.getRotSpeed;

			if (rotationTimer > 1f && inputVec.magnitude < 0.4f)
			{
				keepStretchedOut = false;
			}

			if (rotationTimer > maxRotationTime && keepStretchedOut)
			{
				keepStretchedOut = false;
			}

			//Feint
			if (input.attack && rotationTimer < 0.7f)
			{
				state = State.Defend;
				yield break;
			}

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
			rotationTimer += Time.deltaTime * arms.armAttackState.getRetractSpeed;
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

			case State.AttackRetract:
				rTargetPos = blockPos;
				break;

			case State.Staggered:
				//See StaggerRoutine
				break;
		}
	}

	void Update()
	{
		//Store so it's one frame delayed, so others can check during collision what you were actually doing.
		prevState = state;

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
			rotationTimer = 0f;
			fromRotation = handSideRotation;
			toRotation = targetWindupRotation;

			if (input.attack)
			{
				state = State.WindUp;
				StopAllCoroutines();
				StartCoroutine(WindupRoutine());
			}
		}

		if (state == State.WindedUp)
		{
			rotationTimer = 0f;
			fromRotation = targetWindupRotation;
			toRotation = targetAttackRotation;

			if (energyManager.CanSpendStamina(arms.armAttackState.getStaminaAmount))
			{
				if (!input.attack)
				{
					state = State.Attack;
					StopAllCoroutines();
					StartCoroutine(SwingRoutine());
				}
			}
			else
			{
				state = State.Defend;
			}
		}

		SetTargetPos();

		float zPosBlendSpeedToUse = zPosBlendSpeed;
		float xyPosBlendSpeedToUse = xyPosBlendSpeed;

		if (state == State.StaggeredEnd)
		{
			zPosBlendSpeedToUse /= 6;
			xyPosBlendSpeedToUse *= 0.3f;
		}

		//------------ POSITION ------------\\
		Vector3 localIKPos = rHandIKTarget.localPosition;
		Vector3 localTargetPos = mech.transform.InverseTransformPoint(rTargetPos);
		float xyLerpFactor = Time.deltaTime * xyPosBlendSpeedToUse * scaleFactor;
		
		//Lerp Z position separately
		localIKPos.x = Mathf.Lerp(localIKPos.x, localTargetPos.x, xyLerpFactor);
		localIKPos.y = Mathf.Lerp(localIKPos.y, localTargetPos.y, xyLerpFactor);
		localIKPos.z = Mathf.Lerp(localIKPos.z, localTargetPos.z, Time.deltaTime * zPosBlendSpeedToUse * scaleFactor);

		//Set final position
		rHandIKTarget.localPosition = localIKPos;

		lHandIKTarget.position = lHandTarget.position;

		//------------ ROTATION ------------\\
		Quaternion finalTargetRotation = Quaternion.Lerp(fromRotation, toRotation, rotationTimer);	//Interpolate
		finalRotation = Quaternion.Lerp(finalRotation, finalTargetRotation, Time.deltaTime * rotationBlendSpeed * scaleFactor);   //Smooth

		//Set final rotation
		//rHandIKTarget.localRotation = finalRotation;
		rHandIKTarget.rotation = finalRotation;


		lHandIKTarget.rotation = lHandTarget.rotation;
	}
}