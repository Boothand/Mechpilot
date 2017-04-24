//using System.Collections;
using UnityEngine;

public class Lockon : MechComponent
{
	public bool lockedOn { get; private set; }

	//Events when we lock on to someone.
	public System.Action OnLockOn;
	public System.Action OnLockOff;
	//Events when we get locked on by someone.
	public System.Action OnGetLockedOn;
	public System.Action OnGetLockedOff;

	[SerializeField] bool debugAlwaysLockon;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		if (debugAlwaysLockon)
			input.lockOn = true;

		//Initialize the lock-on.
		if (input.lockOn)
		{
			if (!lockedOn && mech.tempEnemy)
			{
				lockedOn = true;

				if (OnLockOn != null)
					OnLockOn();

				//Play the event on the enemy..
				Lockon enemyLockon = mech.tempEnemy.pilot.lockOn;
				if (enemyLockon.OnGetLockedOn != null)
					enemyLockon.OnGetLockedOn();
			}
			//else if (lockedOn)
			//{
			//	lockedOn = false;

			//	if (OnLockOff != null)
			//		OnLockOff();
			//}
		}

		//When we release the lock-on button, deactivate lock-on.
		if (!input.lockOn &&
			lockedOn)
		{
			lockedOn = false;

			if (OnLockOff != null)
				OnLockOff();

			Lockon enemyLockon = mech.tempEnemy.pilot.lockOn;
			if (enemyLockon != null &&
				enemyLockon.OnGetLockedOff != null)
			{
				enemyLockon.OnGetLockedOff();
			}
		}

		//If the enemy ceases to exist (for us) somehow, deactivate lock-on.
		if (lockedOn && mech.tempEnemy == null)
		{
			lockedOn = false;

			if (OnLockOff != null)
				OnLockOff();

			//Call the event on the enemy..
			Lockon enemyLockon = mech.tempEnemy.pilot.lockOn;
			if (enemyLockon.OnGetLockedOff != null)
				enemyLockon.OnGetLockedOff();
		}
	}
}