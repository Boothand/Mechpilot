using System.Collections;
using UnityEngine;

//The state after attacking while it goes back to stance.
public class Retract : MechComponent
{
	[SerializeField] float retractDuration = 0.75f;
	[SerializeField] float blendTime = 0.5f;
	public bool retracting { get; private set; }

	//Callback when starting to retract.
	public System.Action OnRetractBegin;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	//For cancelling the retract..ion.
	public void Stop()
	{
		retracting = false;
		StopAllCoroutines();
	}

	//The animation to use for retracting
	string AnimFromStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			//case WeaponsOfficer.CombatDir.BottomLeft:
			//	return "Retract BL";
			//case WeaponsOfficer.CombatDir.BottomRight:
			//	return "Retract BR";
			case WeaponsOfficer.CombatDir.Top:
				return "Retract Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Retract TL";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Retract TR";
		}

		return "Unsupported directon";
	}

	IEnumerator RetractRoutine()
	{
		if (OnRetractBegin != null)
			OnRetractBegin();

		retracting = true;

		//Play the retract animation
		//animator.CrossFadeInFixedTime(arms.stancePicker.AnimForStance(arms.stancePicker.stance), blendTime);
		animator.CrossFadeInFixedTime(AnimFromStance(arms.stancePicker.stance), blendTime);

		//Wait the rest of the duration.
		yield return new WaitForSeconds(retractDuration);

		//Go back to idle.
		retracting = false;
		arms.stancePicker.ForceStance(arms.stancePicker.stance);
		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}

	void Update()
	{
		//Initiate the retract routine.
		if (!retracting &&
			arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			StartCoroutine(RetractRoutine());
		}
	}
}