using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	public WeaponsOfficer.CombatDir stance { get; private set; }	//Current stance.
	public WeaponsOfficer.CombatDir prevStance { get; private set; }	//Previous stance, for comparison.
	
	//How long time to use switching stance:
	[SerializeField] float switchTime = 0.5f;
	public float getSwitchTime { get { return switchTime; } }

	//Time to blend into the animation.
	[SerializeField] float blendTime = 0.5f;

	public bool changingStance { get; private set; }
	public WeaponsOfficer.CombatDir startStance;	//Mostly debug.

	//Whether the mech has the sword on left or right side, so we know which idle stance to use.
	public enum Orientation { Right, Left }
	public Orientation orientation { get; set; }
	
	//Callbacks for other classes.
	public System.Action OnStanceBegin;
	public System.Action OnSwitchBegin;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		//Initiate the stance..
		StartCoroutine(ChangeStanceRoutine(startStance));
		animator.CrossFadeInFixedTime(AnimForStance(startStance), blendTime);
	}

	//Get the stance animation
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

		return "Unsupported direction";
	}

	//Cancelling the stance switch
	public override void Stop()
	{
		base.Stop();
		prevStance = stance;
		changingStance = false;
	}

	//Get the orientation anim (left or right)
	public string OrientationAnim()
	{
		if (orientation == Orientation.Left)
			return "Walk/Crouch L";

		return "Walk/Crouch R";
	}

	IEnumerator ChangeStanceRoutine(WeaponsOfficer.CombatDir newStance)
	{
		changingStance = true;

		if (OnSwitchBegin != null)
			OnSwitchBegin();

		//Check if we should switch footing.
		pilot.footStanceSwitcher.CheckSwitchStance(prevStance, stance);

		float switchTimeToUse = switchTime;

		//Play the change stance animation
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

	protected override void OnUpdate()
	{
		//Only update current stance when we're not switching, but regardless of state.
		if (!changingStance)
		{
			stance = arms.DecideCombatDir(stance);
		}

		//Only in the stance state:
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			//Initiate.
			if (arms.prevCombatState != WeaponsOfficer.CombatState.Stance)
			{
				//Run event for others.
				if (OnStanceBegin != null)
					OnStanceBegin();
				
				//Get into the stance pose.
				animator.CrossFadeInFixedTime(AnimForStance(stance), blendTime);
			}

			//If we have picked a different stance than the current one.
			if (!changingStance && prevStance != stance
				&& !arms.blocker.blocking
				)
			{
				StopAllCoroutines();
				StartCoroutine(ChangeStanceRoutine(stance));
			}
		}
	}
}