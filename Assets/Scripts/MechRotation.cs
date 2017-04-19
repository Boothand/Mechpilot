//using System.Collections;
using UnityEngine;

public class MechRotation : MechComponent
{
	public Vector3 getForwardDir { get { return forwardDir;/* aimBaseX.forward;*/ } }
	[SerializeField] float lockonAngleLimit = 60f;
	[SerializeField] float turnSpeed = 60f;
	[SerializeField] float stopTurnSpeed = 10f;
	float rotationAmount;
	float angle;

	Vector3 forwardDir;

	//[SerializeField] bool lockOn = true;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		//Gradually increase/decrease view angle according to input
		if (Mathf.Abs(input.turnBodyHorz) > 0.1f)
		{
			angle += Mathf.Sign(input.turnBodyHorz) * Time.deltaTime * turnSpeed;
		}
		else
		{
			angle = Mathf.Lerp(angle, 0f, Time.deltaTime * stopTurnSpeed);
		}

		//If locked on, look towards the enemy when you're not turning
		if (lockOn.lockedOn
			&& arms.combatState != WeaponsOfficer.CombatState.Attack
			&& mech.tempEnemy)
		{
			forwardDir = mech.tempEnemy.transform.position - mech.transform.position;
			/*forwardDir = Quaternion.Euler(0f, angle, 0f) * forwardDir;*/

			//Don't lock on anymore if you look too much away
			//if (Mathf.Abs(angle) > lockonAngleLimit)
			//{
			//	lockedOn = false;
			//}
		}
		else
		{
			//If not locked on, rotate normally on your own
			forwardDir = Quaternion.Euler(0f, angle, 0f) * mech.transform.forward;
		}

		forwardDir.y = 0f;
		forwardDir.Normalize();

		//Gradually tween mech's forward direction
		mech.transform.forward = Vector3.Slerp(mech.transform.forward, forwardDir, Time.deltaTime * 2f);
		
	}
}