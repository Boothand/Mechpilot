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
		Attack
	}

	public State state { get; private set; }
	
	[Header("Idle/Blocking")]
	[SerializeField] float rotationSpeed = 200f;
	[SerializeField] float idleRotationLimit = 140f;
	float idleTargetAngle;
	Quaternion idleHandRotation;

	[Header("Wind-Up")]
	[SerializeField] float rotateBackAmount = 75;
	[SerializeField] float windupSpeed = 2f;
	Quaternion targetWindupRotation;

	[Header("Attack")]
	[SerializeField] float attackSpeed = 3f;
	[SerializeField] float swingAmount = 120f;
	Quaternion targetAttackRotation;

	[Header("All")]
	[SerializeField] float blendSpeed = 5f;

	Quaternion finalRotation;
	Quaternion fromRotation;
	Quaternion toRotation;
	float rotationTimer;



	protected override void OnAwake()
	{
		base.OnAwake();
		state = State.Defend;
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

		while (input.attack)
		{
			fromRotation = targetWindupRotation;
			toRotation = targetAttackRotation;
			//print(fromRotation.eulerAngles.y + " " + idleTargetAngle);
			yield return null;
		}

		fromRotation = targetWindupRotation;
		toRotation = targetAttackRotation;

		//print("Before: " + idleTargetAngle);
		//idleTargetAngle = -fromRotation.eulerAngles.y;
		//print("After: " + idleTargetAngle);

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

	Quaternion WindUpRotation()
	{
		//Get the direction from the 'middle' to the hand position
		//Transform rHand = arms.armMovement.rHandIK;
		//Vector3 handCenterPos = arms.armMovement.handCenterPos;
		//Vector3 middleToHandDir = rHand.position - handCenterPos;

		////print(rHand.localEulerAngles.y);

		////Use the y angle of the mech to make it the same regardless of orientation
		//float yAngle = mech.transform.eulerAngles.y;
		//Quaternion mechAngle = Quaternion.Euler(0, -yAngle, 0);

		////Rotation aligned with the direction to the center
		//Quaternion towardsMiddleRotation = Quaternion.LookRotation(mechAngle * -middleToHandDir, Vector3.forward);

		//The extra angle to rotate the sword back
		//Quaternion verticalAngle = Quaternion.Euler(-rotateBackAmount, 0, 0);
		//return towardsMiddleRotation * verticalAngle;

		Quaternion verticalAngle = Quaternion.Euler(-rotateBackAmount, 0, 0);
		return idleHandRotation * verticalAngle;
	}

	void Update()
	{
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