using System.Collections;
using UnityEngine;

public class Stagger : MechComponent
{
	[SerializeField] float duration = 1f;
	public bool staggering { get; private set; }
	public float staggerTimer { get; private set; }
	public delegate void NoParam();
	public event NoParam OnStaggerBegin;
	[SerializeField] float blendTime = 0.5f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		healthManager.OnGetHit -= GetHit;
		healthManager.OnGetHit += GetHit;
	}

	void GetHit(Vector3 location)
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

		//Wait 0.3 seconds before returning from the attack, so it stays planted a little bit.
		//if (attacker.dir == WeaponsOfficer.CombatDir.BottomLeft)
			//animator.CrossFade(stancePicker.AnimForStance(stancePicker.stance), stancePicker.getSwitchTime);

		yield return new WaitForSeconds(0.3f);

		//if (attacker.dir != WeaponsOfficer.CombatDir.BottomLeft)
			animator.CrossFadeInFixedTime(stancePicker.AnimForStance(stancePicker.stance), blendTime);

		yield return new WaitForSeconds(durationToUse - 0.3f);

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