using System.Collections;
using UnityEngine;

public class WeaponControl : MechComponent
{
	public enum State
	{
		Idle,
		Defend,
		WindUp,
		Attack
	}

	public State state { get; private set; }
	
	[SerializeField] float rotationSpeed = 200f;
	[SerializeField] float attackSpeed = 10f;
	Quaternion finalRotation;
	Quaternion fromRotation;
	Quaternion targetRotation;
	Quaternion targetWindupRotation;
	Quaternion targetAttackRotation;
	Quaternion handRotation;
	float rotationTimer;
	float idleTargetRot;

	protected override void OnAwake()
	{
		base.OnAwake();
		state = State.Defend;
	}

	Quaternion IdleArmRotation()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);

		//Add input values to the target rotation
		idleTargetRot += rotationInput * Time.deltaTime * rotationSpeed * engineer.energies[ARMS_INDEX];

		//Wrap
		if (idleTargetRot > 360)
			idleTargetRot -= 360f;

		if (idleTargetRot < -360)
			idleTargetRot += 360f;

		//Limit target rotation
		float limit = 140;
		idleTargetRot = Mathf.Clamp(idleTargetRot, -limit, limit);

		//Return the rotation
		Quaternion localRotation = Quaternion.Euler(90, 0, 0) * Quaternion.Euler(0, -idleTargetRot, 0);
		return localRotation;
	}

	IEnumerator SwingRoutine()
	{
		float windupSpeed = 2f;
		float attackSpeed = 2f;
		rotationTimer = 0f;

		//Winding up
		while (rotationTimer < 1f)
		{
			fromRotation = handRotation;
			targetRotation = targetWindupRotation;
			rotationTimer += Time.deltaTime * windupSpeed;
			yield return null;
		}

		rotationTimer = 0f;

		while (input.attack)
		{
			fromRotation = targetWindupRotation;
			targetRotation = targetAttackRotation;
			yield return null;
		}

		fromRotation = targetWindupRotation;
		targetRotation = targetAttackRotation;
		print("Got here");

		//Attack
		state = State.Attack;
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * attackSpeed * 1.5f;
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);

		//Retract
		while (rotationTimer > 0f)
		{
			fromRotation = handRotation;
			targetRotation = targetAttackRotation;
			rotationTimer -= Time.deltaTime * attackSpeed * 1.3f;
			yield return null;
		}

		state = State.Defend;
	}

	void Update()
	{
		handRotation = IdleArmRotation();
		targetWindupRotation = handRotation * Quaternion.Euler(-35, 0, 0);
		targetAttackRotation = handRotation * Quaternion.Euler(90, 0, 0);

		if (state == State.Defend)
		{
			fromRotation = handRotation;
			targetRotation = targetWindupRotation;

			if (input.attack)
			{
				state = State.WindUp;
				StartCoroutine(SwingRoutine());
			}
		}
		
		Quaternion finalTargetRotation = Quaternion.Lerp(fromRotation, targetRotation, rotationTimer);
		finalRotation = Quaternion.Lerp(finalRotation, finalTargetRotation, Time.deltaTime * 5f);

		//Apply the rotation
		Transform rHandIk = arms.armMovement.rHandIK;
		rHandIk.localRotation = finalRotation;
	}
}