using System.Collections;
using UnityEngine;

public class Dasher : MechComponent
{
	public bool inDash { get; private set; }
	[SerializeField] float dashForce = 4f;
	[SerializeField] float staminaUsage = 20f;

	//Store these so they persist until we are ready to apply them to velocity
	//and accelerationSpeed. See ModifyVelAndAcc().
	Vector3 modifiedVelocity;
	float modifiedAccSpeed;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		pilot.movement.ProcessVelAndAcc += ModifyVelAndAcc;
	}

	IEnumerator DashRoutine(Vector3 origVel, System.Action<Vector3> velocity, System.Action<float> accelerationSpeed)
	{
		inDash = true;

		//Map input values to animator blend tree
		animator.SetFloat("DashX", input.moveHorz);
		animator.SetFloat("DashY", input.moveVert);

		//Play the dash animation
		animator.CrossFadeInFixedTime("Dash", 0.2f);

		//Convert the input direction to a world space direction
		Vector3 inputVector = new Vector3(input.moveHorz, 0f, input.moveVert);
		Vector3 worldDashDir = mech.transform.TransformDirection(inputVector).normalized;

		energyManager.SpendStamina(staminaUsage);

		//Use this velocity as an instant force, then decrease it.
		Vector3 newVel = worldDashDir * dashForce;

		float timer = 0f;
		while (timer < 0.4f)
		{
			timer += Time.deltaTime;

			newVel = Vector3.MoveTowards(newVel, origVel, Time.deltaTime * 4f);

			//Return the results:
			velocity(newVel);
			accelerationSpeed(5f);

			yield return null;
		}

		//Finally, make us halt a bit.
		velocity(newVel * 0.3f);

		yield return new WaitForSeconds(0.2f);
		inDash = false;
	}

	//Modifies the velocity and accelerationSpeed before it is applied in MechMovement.cs.
	//Uses lambda syntax to get a callback from the coroutine. Essentially a way to
	//get a return value from the coroutine other than IEnumerator, since we cannot use
	//'out' parameters in coroutines.
	void ModifyVelAndAcc(ref Vector3 velocity, ref float accelerationSpeed)
	{
		//If we press the dash button
		if (input.dash)
		{
			if (energyManager.CanSpendStamina(staminaUsage)
				&& !pilot.croucher.crouching
				&& !pilot.dodger.dodging)
			{
				//If we move the stick
				if (Mathf.Abs(input.moveHorz) > 0.1f ||
					Mathf.Abs(input.moveVert) > 0.1f)
				{

					//Start the dash routine, get the velocity
					//and acceleration speed back from it.
					StopAllCoroutines();
					StartCoroutine(DashRoutine(velocity, (returnedVelocity) =>
					{
						modifiedVelocity = returnedVelocity;
					},
					(returnedAccelerationSpeed) =>
					{
						modifiedAccSpeed = returnedAccelerationSpeed;
					}));
				}
			}
		}

		//When in the routine, apply the modified results.
		if (inDash)
		{
			velocity = modifiedVelocity;
			accelerationSpeed = modifiedAccSpeed;
		}
	}
}