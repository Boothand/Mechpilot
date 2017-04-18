//using System.Collections;
using UnityEngine;

public class FootStanceSwitcher : MechComponent
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		//stancePicker.OnSwitchBegin += SwitchStance;
		//blocker.OnBlockBegin += SwitchStance;
	}

	public void CheckSwitchStance(WeaponsOfficer.CombatDir prev, WeaponsOfficer.CombatDir current)
	{
		if (!pilot.move.moving)
		{
			if (prev == WeaponsOfficer.CombatDir.TopLeft
				&& current == WeaponsOfficer.CombatDir.TopRight)
			{
				animator.CrossFadeInFixedTime("Idle Switch L2R", 0.15f);
				stancePicker.orientation = StancePicker.Orientation.Right;
			}
			else if (prev == WeaponsOfficer.CombatDir.TopRight
				&& current == WeaponsOfficer.CombatDir.TopLeft)
			{
				animator.CrossFadeInFixedTime("Idle Switch R2L", 0.15f);
				stancePicker.orientation = StancePicker.Orientation.Left;
			}

			animator.SetInteger("Orientation", (int)stancePicker.orientation);
		}
	}

	void Update()
	{
		
	}
}