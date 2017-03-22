using System.Collections;
using UnityEngine;

public class Stagger : MechComponent
{
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

		float durationToUse = duration;// / 2;
		durationToUse *= durationModifier;

		//Let's try without animating for now, so it stays planted on them.
		//animator.CrossFade(stancePicker.AnimForStance(stancePicker.stance), stancePicker.getSwitchTime);

		yield return new WaitForSeconds(durationToUse);

		WeaponsOfficer.CombatDir stanceToUse = stancePicker.stance;

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