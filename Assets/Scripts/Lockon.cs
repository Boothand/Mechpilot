//using System.Collections;
using UnityEngine;

public class Lockon : MechComponent
{
	public bool lockedOn { get; private set; }

	public System.Action OnLockOn;
	public System.Action OnLockOff;
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

		if (input.lockOn)
		{
			if (!lockedOn && mech.tempEnemy)
			{
				lockedOn = true;

				if (OnLockOn != null)
					OnLockOn();

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

		if (lockedOn && mech.tempEnemy == null)
		{
			lockedOn = false;

			if (OnLockOff != null)
				OnLockOff();

			Lockon enemyLockon = mech.tempEnemy.pilot.lockOn;
			if (enemyLockon.OnGetLockedOff != null)
				enemyLockon.OnGetLockedOff();
		}
	}
}