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

	string AnimFromDir(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.TopRight:
				if (croucher.crouching)
					return "Crouch Idle Switch L2R";

				return "Idle Switch L2R";

			case WeaponsOfficer.CombatDir.TopLeft:
				if (croucher.crouching)
					return "Crouch Idle Switch R2L";

				return "Idle Switch R2L";
		}

		return null;
	}

	public void CheckSwitchStance(WeaponsOfficer.CombatDir prev, WeaponsOfficer.CombatDir current)
	{
		if (!pilot.move.moving)
		{
			string animToUse = AnimFromDir(current);

			if (prev == WeaponsOfficer.CombatDir.TopLeft
				&& current == WeaponsOfficer.CombatDir.TopRight)
			{
				animator.CrossFadeInFixedTime(animToUse, 0.15f);
				stancePicker.orientation = StancePicker.Orientation.Right;
			}
			else if (prev == WeaponsOfficer.CombatDir.TopRight
				&& current == WeaponsOfficer.CombatDir.TopLeft)
			{
				animator.CrossFadeInFixedTime(animToUse, 0.15f);
				stancePicker.orientation = StancePicker.Orientation.Left;
			}

			animator.SetInteger("Orientation", (int)stancePicker.orientation);
		}
	}

	void Update()
	{
		
	}
}