using System.Collections;
using UnityEngine;

public class Dasher : MechComponent
{
	public bool inDash { get; private set; }
	[SerializeField] float dashForce = 20f;
	[SerializeField] float staminaUsage = 20f;
	Vector3 vel;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	IEnumerator DashRoutine(Vector3 origVel, System.Action<Vector3> velocity)
	{
		inDash = true;
		Vector3 newVel = Vector3.zero;

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
			yield return null;
		}

		velocity(newVel);
		inDash = false;
	}

	public void RunComponent(ref Vector3 velocity)
	{
		if (input.dash)
		{
			//inDash = true;
			if (energyManager.CanSpendStamina(staminaUsage))
			{
				if (Mathf.Abs(input.moveHorz) > 0.1f ||
					Mathf.Abs(input.moveVert) > 0.1f)
				{
					StopAllCoroutines();
					StartCoroutine(DashRoutine(velocity, (returnValue) =>
					{
						vel = returnValue;
					}));
				}
			}
		}

		if (inDash)
		{
			velocity = vel;
		}
	}
	
	void Update()
	{
		
	}
}