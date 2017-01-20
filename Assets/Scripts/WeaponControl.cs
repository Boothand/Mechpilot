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

		//Attack
		state = State.Attack;
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime * attackSpeed * 1.5f;
			yield return null;
		}

		//yield return new WaitForSeconds(0.5f);

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

	Quaternion WindUpRotation()
	{
		//Get the direction from the 'middle' to the hand position
		Transform rHand = arms.armMovement.rHandIK;
		Vector3 handCenterPos = arms.armMovement.handCenterPos;
		Vector3 middleToHandDir = rHand.position - handCenterPos;
		
		//Use the y angle of the mech to make it the same regardless of orientation
		float yAngle = mech.transform.eulerAngles.y;
		Quaternion mechAngle = Quaternion.Euler(0, -yAngle, 0);

		//Rotation aligned with the direction to the center
		Quaternion towardsMiddleRotation = Quaternion.LookRotation(mechAngle * -middleToHandDir, Vector3.forward);

		//The extra angle to rotate the sword back
		Quaternion verticalAngle = Quaternion.Euler(-75, 0, 0);

		return towardsMiddleRotation * verticalAngle;
	}

	void Update()
	{
		handRotation = IdleArmRotation();

		targetWindupRotation = WindUpRotation();
		Quaternion swingAngle = Quaternion.Euler(120, 0, 0);
		targetAttackRotation = targetWindupRotation * swingAngle;

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