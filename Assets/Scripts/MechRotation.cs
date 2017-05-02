//using System.Collections;
using UnityEngine;

//This component is responsible for making the mech face the right direction.
//If the mech is 'locked on', it faces the closest opponent, if not it can turn freely.
public class MechRotation : MechComponent
{
	[SerializeField] float turnSpeed = 60f;	//Multiplier for turning
	[SerializeField] float stopTurnSpeed = 10f;	//Multiplier when stopping turning

	//Blend speeds for turning
	[SerializeField] float blendSpeed = 2f;
	[SerializeField] float lockonBlendSpeed = 1f;

	bool turning;

	//How many degrees to rotate per second, before any multipliers.
	float angle;

	//Interpolated forward direction
	Vector3 forwardDir;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnUpdate()
	{
		//Gradually increase/decrease view angle according to input.
		if (Mathf.Abs(input.turnBodyHorz) > 0.1f)
		{
			float turnInput = Mathf.Clamp(input.turnBodyHorz, -1f, 1f);
			angle += turnInput * Time.deltaTime * turnSpeed;

			if (!turning && !pilot.move.moving)
			{
				animator.CrossFadeInFixedTime("Idle Switch R2L", 0.25f);
			}

			turning = true;
		}
		else
		{
			//Lower threshold, tween angle back to 0 if no input.
			angle = Mathf.Lerp(angle, 0f, Time.deltaTime * stopTurnSpeed);

			turning = false;
		}

		//If you turn too fast, things will go horribly wrong, so make sure
		//angle stays at a reasonable level.
		angle = Mathf.Clamp(angle, -30f, 30f);


		//If locked on, look towards the enemy when you're not turning.
		if (pilot.lockOn.lockedOn
			&& arms.combatState != WeaponsOfficer.CombatState.Attack
			&& mech.tempEnemy)
		{
			forwardDir = mech.tempEnemy.transform.position - mech.transform.position;
		}
		else
		{
			//If not locked on or if you're attacking, rotate normally on your own.
			forwardDir = Quaternion.Euler(0f, angle, 0f) * mech.transform.forward;
		}

		//Keep the forward direction straight, so we never tilt.
		forwardDir.y = 0f;
		forwardDir.Normalize();

		//Gradually tween mech's forward direction
		float blendSpeedToUse = blendSpeed;

		//Turn a bit slower if using only lock-on, seems fair to not have perfect tracking of the opponent.
		if (pilot.lockOn.lockedOn)
			blendSpeedToUse = lockonBlendSpeed;

		//Tween to the final forward direction
		mech.transform.forward = Vector3.Slerp(mech.transform.forward, forwardDir, Time.deltaTime * blendSpeedToUse);
	}
}