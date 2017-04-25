using System.Collections;
using UnityEngine;

//The state after you have been blocked or hit someone.
public class Stagger : MechComponent
{
	[SerializeField] float duration = 1f;
	public bool staggering { get; private set; }
	public float staggerTimer { get; private set; }
	public System.Action OnStaggerBegin;
	[SerializeField] float blendTime = 0.5f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		healthManager.OnGetHit += GetHit;
	}

	//Getting hit also sends you into a stagger state?
	void GetHit(Vector3 location)
	{
		arms.stancePicker.Stop();
		arms.windup.Stop();
		arms.attacker.Stop();
		arms.retract.Stop();
		arms.blocker.Stop();
		GetStaggered(arms.stancePicker.stance);
	}

	//For cancelling the stagger.
	public override void Stop()
	{
		base.Stop();
		staggering = false;
	}

	IEnumerator StaggerRoutine(WeaponsOfficer.CombatDir dir, float durationModifier = 1f)
	{
		if (OnStaggerBegin != null)
			OnStaggerBegin();

		arms.combatState = WeaponsOfficer.CombatState.Stagger;
		staggering = true;

		float durationToUse = duration;
		durationToUse *= durationModifier;

		staggerTimer = 0f;	//This is read by other classes.

		//Wait 0.3 seconds before returning from the attack, so it stays planted a little bit.
		float plantedTime = 0.3f;
		while (staggerTimer < plantedTime)
		{
			staggerTimer += Time.deltaTime;
			yield return null;
		}
		
		//Go to the stance animation in the end.
		animator.CrossFadeInFixedTime(arms.stancePicker.AnimForStance(arms.stancePicker.stance), blendTime);

		//Wait the rest of the duration minus the other wait times.
		while (staggerTimer < durationToUse - plantedTime)
		{
			staggerTimer += Time.deltaTime;
			yield return null;
		}

		//Return to stance.
		arms.combatState = WeaponsOfficer.CombatState.Stance;
		arms.stancePicker.ForceStance(arms.stancePicker.stance);

		staggering = false;
	}

	//Initiate the stagger routine.
	public void GetStaggered(WeaponsOfficer.CombatDir dir, float durationModifier = 1f)
	{
		StopAllCoroutines();
		StartCoroutine(StaggerRoutine(dir, durationModifier));
	}
}