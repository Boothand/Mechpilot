using System.Collections;
using UnityEngine;

public class Stagger : MechComponent
{
	[SerializeField] IKPose tlPose, trPose, blPose, brPose, topPose;
	[SerializeField] float duration = 1f;
	public bool staggering { get; private set; }
	public float staggerTimer { get; private set; }
	public delegate void NoParam();
	public event NoParam OnStaggerBegin;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		healthManager.OnGetHit -= GetHit;
		healthManager.OnGetHit += GetHit;
	}

	void GetHit()
	{
		stancePicker.Stop();
		windup.Stop();
		attacker.Stop();
		retract.Stop();
		blocker.Stop();
		GetStaggered(stancePicker.stance);
	}

	IKPose GetPose(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return blPose;
			case WeaponsOfficer.CombatDir.BottomRight:
				return brPose;
			case WeaponsOfficer.CombatDir.Top:
				return topPose;
			case WeaponsOfficer.CombatDir.TopLeft:
				return tlPose;
			case WeaponsOfficer.CombatDir.TopRight:
				return trPose;
		}

		return topPose;
	}

	public void Stop()
	{
		StopAllCoroutines();
		staggering = false;
	}

	IEnumerator StaggerRoutine(WeaponsOfficer.CombatDir dir, float durationModifier = 1f)
	{
		if (OnStaggerBegin != null)
			OnStaggerBegin();

		arms.combatState = WeaponsOfficer.CombatState.Stagger;
		staggering = true;
		//arms.StoreTargets();
		//IKPose targetPose = GetPose(dir);
		//IKPose targetPose2 = stancePicker.GetStancePose(stancePicker.stance);

		//float timer = 0f;

		float durationToUse = duration;// / 2;
		durationToUse *= durationModifier;

		animator.CrossFade(stancePicker.AnimForStance(stancePicker.stance), stancePicker.getSwitchTime);

		yield return new WaitForSeconds(durationToUse);
		//while (timer < durationToUse)
		//{
		//	timer += Time.deltaTime;
		//	staggerTimer = timer / durationToUse / 2;

		//	arms.InterpolateIKPose(targetPose, timer / durationToUse);
		//	//print(timer / duration);
		//	yield return null;
		//}

		//timer = 0f;
		//arms.StoreTargets();
		//targetPose2 = stancePicker.GetStancePose(stancePicker.stance);
		//float lastStaggerTimer = staggerTimer;
		WeaponsOfficer.CombatDir stanceToUse = stancePicker.stance;

		//while (timer < durationToUse)
		//{
		//	timer += Time.deltaTime;

		//	staggerTimer = timer / durationToUse / 2 + lastStaggerTimer;
		//	arms.InterpolateIKPose(targetPose2, timer / durationToUse);
		//	yield return null;
		//}

		arms.combatState = WeaponsOfficer.CombatState.Stance;
		stancePicker.ForceStance(stanceToUse);

		staggering = false;
	}

	public void GetStaggered(WeaponsOfficer.CombatDir dir, float durationModifier = 1f)
	{
		StopAllCoroutines();
		StartCoroutine(StaggerRoutine(dir, durationModifier));
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stagger)
		{

		}
	}
}