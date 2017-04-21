using System.Collections;
using UnityEngine;

public class Dasher : MechComponent
{
	public bool inDash { get; private set; }
	[SerializeField] float dashForce = 4f;
	[SerializeField] float staminaUsage = 20f;
	Vector3 vel;
	float accSpeed;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	IEnumerator DashRoutine(Vector3 origVel, System.Action<Vector3> velocity, System.Action<float> accelerationSpeed)
	{
		inDash = true;
		Vector3 newVel = Vector3.zero;
		animator.SetFloat("DashX", input.moveHorz);
		animator.SetFloat("DashY", input.moveVert);

		animator.CrossFadeInFixedTime("Dash", 0.2f);
		Vector3 inputVector = new Vector3(input.moveHorz, 0f, input.moveVert);
		Vector3 worldDashDir = mech.transform.TransformDirection(inputVector).normalized;
		newVel += worldDashDir * dashForce;

		energyManager.SpendStamina(staminaUsage);

		float timer = 0f;
		while (timer < 0.4f)
		{
			timer += Time.deltaTime;

			newVel = Vector3.MoveTowards(newVel, origVel, Time.deltaTime * 4f);
			velocity(newVel);
			accelerationSpeed(5f);
			yield return null;
		}

		velocity(newVel * 0.3f);
		//velocity(newVel);
		yield return new WaitForSeconds(0.2f);
		inDash = false;
	}

	public void ModifyVelAndAcc(ref Vector3 velocity, ref float accelerationSpeed)
	{
		if (input.dash)
		{
			//inDash = true;
			if (energyManager.CanSpendStamina(staminaUsage)
				&& pilot.croucher.animCrouchHeight < 0.5f
				&& !pilot.dodger.dodging)
			{
				if (Mathf.Abs(input.moveHorz) > 0.1f ||
					Mathf.Abs(input.moveVert) > 0.1f)
				{
					StopAllCoroutines();
					StartCoroutine(DashRoutine(velocity, (returnValue) =>
					{
						vel = returnValue;
					},
					(returnValue2) =>
					{
						accSpeed = returnValue2;
					}));
				}
			}
		}

		if (inDash)
		{
			velocity = vel;
			accelerationSpeed = accSpeed;
		}
	}
	
	void Update()
	{
		
	}
}