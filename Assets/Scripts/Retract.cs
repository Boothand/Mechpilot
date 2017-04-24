﻿using System.Collections;
using UnityEngine;

public class Retract : MechComponent
{
	[SerializeField] float retractDuration = 0.75f;
	[SerializeField] float blendTime = 0.5f;
	public bool retracting { get; private set; }
	public delegate void NoParam();
	public event NoParam OnRetractBegin;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void Stop()
	{
		retracting = false;
		StopAllCoroutines();
	}

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
		//print("Begin retract");
		if (OnRetractBegin != null)
			OnRetractBegin();

		retracting = true;

		//animator.CrossFadeInFixedTime(arms.stancePicker.AnimForStance(arms.stancePicker.stance), blendTime);
		animator.CrossFadeInFixedTime(AnimFromStance(arms.stancePicker.stance), blendTime);

		//WeaponsOfficer.CombatDir stanceToUse = stancePicker.stance;

		yield return new WaitForSeconds(retractDuration);

		retracting = false;
		arms.stancePicker.ForceStance(arms.stancePicker.stance);
		arms.combatState = WeaponsOfficer.CombatState.Stance;
		//print("End retract");
		yield return null;
	}

	void Update()
	{
		if (!retracting && arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			StartCoroutine(RetractRoutine());
		}
	}
}