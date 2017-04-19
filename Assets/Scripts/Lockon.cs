//using System.Collections;
using UnityEngine;

public class Lockon : MechComponent
{
	public bool lockedOn { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		if (input.lockOn)
		{
			if (!lockedOn && mech.tempEnemy)
			{
				lockedOn = true;
			}
			else if (lockedOn)
			{
				lockedOn = false;
			}
		}

		if (mech.tempEnemy == null)
		{
			lockedOn = false;
		}
	}
}