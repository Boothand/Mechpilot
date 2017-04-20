//using System.Collections;
using UnityEngine;

public class Lockon : MechComponent
{
	public bool lockedOn { get; private set; }

	public System.Action OnLockOn;
	public System.Action OnLockOff;

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
		}

		if (lockedOn && mech.tempEnemy == null)
		{
			lockedOn = false;

			if (OnLockOff != null)
				OnLockOff();
		}
	}
}