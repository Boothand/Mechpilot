//using System.Collections;
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

	State state = State.Defend;
	
	[SerializeField] float rotationSpeed = 200f;
	Quaternion handRotation;
	float attackTimer;
	float idleTargetRot;
	float idleHandRotation;

	protected override void OnAwake()
	{
		base.OnAwake();
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

		//Smoothly interpolate hand rotation
		idleHandRotation = Mathf.LerpAngle(idleHandRotation, idleTargetRot, Time.deltaTime * 5f);

		//Limit hand rotation
		idleHandRotation = Mathf.Clamp(idleHandRotation, -limit, limit);

		//Return the rotation
		Quaternion localRotation = Quaternion.Euler(70, 0, 0) * Quaternion.Euler(0, -idleHandRotation, 0);
		return localRotation;
	}

	void Update()
	{
		switch (state)
		{
			case State.Idle:

				break;

			case State.Defend:

				handRotation = IdleArmRotation();

				if (input.attack)
				{
					state = State.Attack;
				}

				break;

			case State.WindUp:

				

				if (!input.attack)
				{
					state = State.Defend;
				}

				break;

			case State.Attack:

				Quaternion attackTargetRot = handRotation * Quaternion.Euler(0, 90, 0);


				float attackDuration = 1f;

				attackTimer += Time.deltaTime;

				if (attackTimer > attackDuration)
				{
					attackTimer = 0f;
					state = State.Defend;
				}

				if (!input.attack)
				{
					state = State.Defend;
					attackTimer = 0f;
				}

				break;
		}

		//Apply the rotation
		Transform rHandIk = arms.armMovement.rHandIK;
		rHandIk.localRotation = handRotation;
	}
}