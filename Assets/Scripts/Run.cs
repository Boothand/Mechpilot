//using System.Collections;
using UnityEngine;

public class Run : MechComponent
{
	public bool running { get; private set; }
	public bool inRunCooldown { get; private set; }
	[SerializeField] float runMultiplier = 1.75f;
	[SerializeField] float staminaPerSecond = 15f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		pilot.move.ProcessWorldMoveDir += CheckRun;
	}

	void CheckRun(ref Vector3 worldMoveDir)
	{
		running = false;
		float staminaAmount = staminaPerSecond * Time.deltaTime;

		if (
			//!lockOn.lockedOn &&
			input.run &&
			!inRunCooldown &&
			!croucher.crouching &&
			pilot.move.inputVecMagnitude > 0.2f
			/*&& input.moveVert > 0.2f
			&& Mathf.Abs(input.moveHorz) < 0.3f*/)
		{
			running = true;

			float runMultiplierToUse = runMultiplier;

			Vector3 localVelocity = mech.transform.InverseTransformDirection(pilot.move.getWorldMoveDir);

			if (Mathf.Abs(localVelocity.x) > 0.5f)   //Going sideways
			{
				runMultiplierToUse *= 0.8f; //Don't run as fast as forward
			}
			else if (localVelocity.z < 0f)  //Going backwards
			{
				runMultiplierToUse *= 0.6f; //Don't run as fast as forward
			}
			//print(runMultiplierToUse);

			worldMoveDir *= runMultiplierToUse;// * inputVecMagnitude;

			energyManager.SpendStamina(staminaAmount * runMultiplierToUse);

			if (energyManager.stamina < 0.01f)
			{
				inRunCooldown = true;
			}
		}

		if (inRunCooldown)
		{
			if (energyManager.stamina > 25f)
			{
				inRunCooldown = false;
			}
		}

		float animSpeed = rb.velocity.magnitude / 60;
		//print(animSpeed);

		animator.SetBool("Running", running);
		animator.SetFloat("RunAmount", animSpeed);
	}

	void Update()
	{
		
	}
}