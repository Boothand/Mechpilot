using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	public WeaponsOfficer.CombatDir stance { get; private set; }
	public WeaponsOfficer.CombatDir prevStance { get; private set; }
	WeaponsOfficer.CombatState prevState;

	//[SerializeField] float blendSpeed = 4f;
	[SerializeField] float switchTime = 0.5f;
	[SerializeField] float blendTime = 0.5f;
	public float getSwitchTime { get { return switchTime; } }
	public bool changingStance { get; private set; }
	public WeaponsOfficer.CombatDir startStance;

	public enum Orientation { Right, Left }
	public Orientation orientation { get; set; }
	public Orientation prevOrientation { get; private set; }
	
	public System.Action OnStanceBegin;
	public System.Action OnSwitchBegin;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		stance = startStance;
		StartCoroutine(ChangeStanceRoutine(stance));
		animator.CrossFadeInFixedTime(AnimForStance(stance), blendTime);
	}

	public string AnimForStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			//case WeaponsOfficer.CombatDir.BottomLeft:
			//	return "Stance_BL";
			//case WeaponsOfficer.CombatDir.BottomRight:
			//	return "Stance_BR";
			case WeaponsOfficer.CombatDir.Top:
				return "Stance_Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Stance_TL";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Stance_TR";
		}

		return "Stance_TR";
	}

	public void Stop()
	{
		StopAllCoroutines();
		prevStance = stance;
		changingStance = false;
	}

	public string OrientationAnim()
	{
		if (orientation == Orientation.Left)
			return "Walk/Crouch L";

		return "Walk/Crouch R";
	}

	IEnumerator ChangeStanceRoutine(WeaponsOfficer.CombatDir newStance)
	{
		changingStance = true;

		prevOrientation = orientation;

		if (OnSwitchBegin != null)
			OnSwitchBegin();

		pilot.footStanceSwitcher.CheckSwitchStance(prevStance, stance);

		float switchTimeToUse = switchTime;

		animator.CrossFadeInFixedTime(AnimForStance(stance), blendTime);

		yield return new WaitForSeconds(switchTimeToUse);

		prevStance = stance;
		changingStance = false;
	}
	

	public void ForceStance(WeaponsOfficer.CombatDir newStance)
	{
		stance = newStance;
		prevStance = newStance;
	}

	void Update()
	{
		if (!changingStance)
		{
			stance = arms.DecideCombatDir(stance);
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			//Run event for others
			if (prevState != WeaponsOfficer.CombatState.Stance)
			{
				if (OnStanceBegin != null)
					OnStanceBegin();

				//print("OK");
				animator.CrossFadeInFixedTime(AnimForStance(stance), blendTime);
			}

			if (!changingStance && prevStance != stance
				&& !arms.blocker.blocking
				)
			{
				StopAllCoroutines();
				StartCoroutine(ChangeStanceRoutine(stance));
			}
		}

		prevState = arms.combatState;
	}
}