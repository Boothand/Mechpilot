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

	protected override void OnStart()
	{
		base.OnStart();

		//Modify velocity before it is applied.
		pilot.movement.ProcessWorldMoveDir += CheckRun;
	}

	void CheckRun(ref Vector3 worldMoveDir)
	{
		running = false;

		//If pressing 'run' and you're allowed to run:
		if (
			input.run &&
			!inRunCooldown &&
			!pilot.croucher.crouching &&
			pilot.movement.inputVecMagnitude > 0.2f
			//!lockOn.lockedOn &&
			/*&& input.moveVert > 0.2f
			&& Mathf.Abs(input.moveHorz) < 0.3f*/)
		{
			running = true;

			float runMultiplierToUse = runMultiplier;

			Vector3 localVelocity = mech.transform.InverseTransformDirection(pilot.movement.getWorldMoveDir);

			if (Mathf.Abs(localVelocity.x) > 0.5f)   //Going sideways
			{
				runMultiplierToUse *= 0.8f; //Don't run as fast as forward
			}
			else if (localVelocity.z < 0f)  //Going backwards
			{
				runMultiplierToUse *= 0.8f; //Don't run as fast as forward
			}

			//Apply the modification
			worldMoveDir *= runMultiplierToUse;

			//Spend some stamina every frame you run
			float staminaAmount = staminaPerSecond * runMultiplierToUse * Time.deltaTime;
			energyManager.SpendStamina(staminaAmount);

			//If you run out of stamina, don't run every other frame as you regenerate.
			//Instead, set a cooldown.
			if (energyManager.stamina < 0.01f)
			{
				inRunCooldown = true;
			}
		}

		//Wait until you have 25 stamina before you're allowed to run again
		//after a cooldown
		if (inRunCooldown)
		{
			if (energyManager.stamina > 25f)
			{
				inRunCooldown = false;
			}
		}

		//Temporary stuff until run anims.
		//float animSpeed = rb.velocity.magnitude / 60;

		animator.SetBool("Running", running);
		//animator.SetFloat("RunAmount", animSpeed);
	}
}