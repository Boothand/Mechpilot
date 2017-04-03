using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	public WeaponsOfficer.CombatDir stance { get; private set; }
	public WeaponsOfficer.CombatDir prevStance { get; private set; }
	WeaponsOfficer.CombatState prevState;

	//[SerializeField] float blendSpeed = 4f;
	[SerializeField] float switchTime = 0.5f;
	public float getSwitchTime { get { return switchTime; } }
	public bool changingStance { get; private set; }
	public WeaponsOfficer.CombatDir startStance;

	public delegate void NoParam();
	public event NoParam OnStanceBegin;
	

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		stance = startStance;
		StartCoroutine(ChangeStanceRoutine(stance));
		animator.CrossFade(AnimForStance(stance), 0.25f);
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

	IEnumerator ChangeStanceRoutine(WeaponsOfficer.CombatDir newStance)
	{
		changingStance = true;

		float switchTimeToUse = switchTime;


		animator.CrossFade(AnimForStance(stance), switchTimeToUse);

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
				animator.CrossFade(AnimForStance(stance), switchTime);
			}

			if (!changingStance && prevStance != stance
				&& !blocker.blocking
				)
			{
				StopAllCoroutines();
				StartCoroutine(ChangeStanceRoutine(stance));
			}
		}

		prevState = arms.combatState;
	}
}