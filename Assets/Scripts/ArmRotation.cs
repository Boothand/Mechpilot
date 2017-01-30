using System.Collections;
using UnityEngine;

public class ArmRotation : MechComponent
{
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
	[SerializeField] float rotationSpeed = 200f;
	[SerializeField] float idleRotationLimit = 140f;
	public float idleTargetAngle { get; private set; }
	public float getIdleRotationLimit { get { return idleRotationLimit; } }
	Quaternion idleHandRotation;

	[Header("Wind-Up")]
	[SerializeField] float rotateBackAmount = 75;
	[SerializeField] float windupSpeed = 2f;
	Quaternion targetWindupRotation;

	[Header("Attack")]
	[SerializeField] float attackSpeed = 3f;
	[SerializeField] float swingAmount = 120f;
	Quaternion targetAttackRotation;

	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip[] swordClips;

	[Header("All")]
	[SerializeField] float blendSpeed = 5f;

	[SerializeField] Transform swordTip;
	[SerializeField] Transform test;

	Quaternion finalRotation;
	Quaternion fromRotation;
	Quaternion toRotation;
	float rotationTimer;



	protected override void OnAwake()
	{
		base.OnAwake();
		state = State.Defend;

		arms.getWeapon.GetComponent<Collidable>().OnCollision -= SwordCollide;
		arms.getWeapon.GetComponent<Collidable>().OnCollision += SwordCollide;
	}



	void SwordCollide(Collision col)
	{
		if (col.transform.GetComponent<Collidable>())
		{
			float impact = col.relativeVelocity.magnitude;

			AudioClip randomClip = swordClips[Random.Range(0, swordClips.Length)];
			audioSource.volume = impact / 4f;
			audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
			audioSource.PlayOneShot(randomClip);

			if (impact > 1.3f)
			{
				print("Hit was strong enough");
				Debug.DrawRay(col.contacts[0].point, col.relativeVelocity, Color.red);
				if (state == State.Attack)
				{
					state = State.Staggered;
					StopAllCoroutines();
					StartCoroutine(StaggerRoutine(col.relativeVelocity));
				}
			}
		}
	}

	Quaternion IdleArmRotation()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);

		//Add input values to the target rotation
		idleTargetAngle += rotationInput * Time.deltaTime * rotationSpeed * energyManager.energies[ARMS_INDEX];

		//Wrap
		if (idleTargetAngle > 360)
			idleTargetAngle -= 360f;

		if (idleTargetAngle < -360)
			idleTargetAngle += 360f;

		//Limit target rotation
		idleTargetAngle = Mathf.Clamp(idleTargetAngle, -idleRotationLimit, idleRotationLimit);

		//Return the rotation
		Quaternion localRotation = Quaternion.Euler(90, 0, 0) * Quaternion.Euler(0, -idleTargetAngle, 0);
		return localRotation;
	}

	IEnumerator SwingRoutine()
	{
		rotationTimer = 0f;

		//Winding up
		while (rotationTimer < 1f)
		{
			fromRotation = idleHandRotation;
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

		//Releasing
		//Attack
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
			fromRotation = idleHandRotation;
			toRotation = targetAttackRotation;
			rotationTimer -= Time.deltaTime * attackSpeed * 1.3f * energyManager.energies[ARMS_INDEX];
			yield return null;
		}

		state = State.Defend;
	}

	IEnumerator StaggerRoutine(Vector3 velocity)
	{
		Transform rHandIK = arms.armMovement.rHandIK;
		Vector3 newTipPos = swordTip.position + velocity * 0.1f;

		Vector3 dir = (rHandIK.position - rHandIK.position).normalized;
		Quaternion newRot = Quaternion.LookRotation(-dir);

		fromRotation = rHandIK.localRotation;
		toRotation = newRot;

		rotationTimer = 0f;
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * 4f;
			yield return null;
		}

		rotationTimer = 0f;

		fromRotation = newRot;
		toRotation = idleHandRotation;

		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * 4f;
			yield return null;
		}
		
		state = State.Defend;
	}

	Quaternion WindUpRotation()
	{
		Quaternion verticalAngle = Quaternion.Euler(-rotateBackAmount, 0, 0);
		return idleHandRotation * verticalAngle;
	}

	void Update()
	{
		//Vector3 dir = (test.position - arms.armMovement.rHandIK.position).normalized;

		//Quaternion rot = Quaternion.LookRotation(-dir);
		//arms.armMovement.rHandIK.localRotation = rot;

		//return;
		idleHandRotation = IdleArmRotation();

		targetWindupRotation = WindUpRotation();
		Quaternion swingAngle = Quaternion.Euler(swingAmount, 0, 0);
		targetAttackRotation = targetWindupRotation * swingAngle;

		if (state == State.Defend)
		{
			fromRotation = idleHandRotation;
			toRotation = targetWindupRotation;

			if (input.attack)
			{
				state = State.WindUp;
				StartCoroutine(SwingRoutine());
			}
		}

		Quaternion finalTargetRotation = Quaternion.Lerp(fromRotation, toRotation, rotationTimer);
		finalRotation = Quaternion.Lerp(finalRotation, finalTargetRotation, Time.deltaTime * blendSpeed);

		//Apply the rotation
		Transform rHandIk = arms.armMovement.rHandIK;
		rHandIk.localRotation = finalRotation;
	}
}