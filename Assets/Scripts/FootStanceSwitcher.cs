//using System.Collections;
using UnityEngine;

public class FootStanceSwitcher : MechComponent
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	string AnimFromDir(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.TopRight:

				if (pilot.croucher.crouching)
					return "Crouch Idle Switch L2R";

				return "Idle Switch L2R";

			case WeaponsOfficer.CombatDir.TopLeft:

				if (pilot.croucher.crouching)
					return "Crouch Idle Switch R2L";

				return "Idle Switch R2L";
		}

		return null;
	}

	//Compares your stances, plays a foot switching anim if they are opposing directions.
	//Updates 'orientation' so animator knows which orientation stance (left/right) to transition to.
	public void CheckSwitchStance(WeaponsOfficer.CombatDir prev, WeaponsOfficer.CombatDir current)
	{
		if (!pilot.movement.moving)
		{
			string animToUse = AnimFromDir(current);

			if (prev == WeaponsOfficer.CombatDir.TopLeft
				&& current == WeaponsOfficer.CombatDir.TopRight)
			{
				animator.CrossFadeInFixedTime(animToUse, 0.15f);
				arms.stancePicker.orientation = StancePicker.Orientation.Right;
			}
			else if (prev == WeaponsOfficer.CombatDir.TopRight
				&& current == WeaponsOfficer.CombatDir.TopLeft)
			{
				animator.CrossFadeInFixedTime(animToUse, 0.15f);
				arms.stancePicker.orientation = StancePicker.Orientation.Left;
			}

			animator.SetInteger("Orientation", (int)arms.stancePicker.orientation);
		}
	}
}