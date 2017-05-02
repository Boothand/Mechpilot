using System.Collections;
using UnityEngine;

public class Windup : MechComponent
{
	[SerializeField] float windupTime = 0.5f;
	public bool windingUp { get; private set; }
	public WeaponsOfficer.CombatDir dir { get; private set; }

	//For queueing a windup, for instance if you press attack when switching stance:
	bool cachedAttack;
	public float windupTimer { get; private set; }

	public System.Action OnWindupBegin;	//Callback when you start winding up.
	public bool inCounterAttack { get; private set; }
	[SerializeField] float blendTime = 0.1f;	//Blend time for entering the animation.


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	//Get the correct windup animation:
	string AnimFromStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			//case WeaponsOfficer.CombatDir.BottomLeft:
			//	return "Windup Bottom Left";
			//case WeaponsOfficer.CombatDir.BottomRight:
			//	return "Windup Bottom Right";
			case WeaponsOfficer.CombatDir.Top:
				return "Windup Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Windup Top Left";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Windup Top Right";
		}

		return "Unsupported direction";
	}

	//Cancelling the windup:
	public override void Stop()
	{
		base.Stop();
		windingUp = false;
		cachedAttack = false;
	}

	IEnumerator WindupRoutine(WeaponsOfficer.CombatDir dir)
	{
		if (OnWindupBegin != null)
			OnWindupBegin();

		windingUp = true;
		arms.combatState = WeaponsOfficer.CombatState.Windup;

		windupTimer = 0f;	//Only relevant for others.

		//Enter the animation
		animator.CrossFadeInFixedTime(AnimFromStance(arms.stancePicker.stance), blendTime);

		//Wait the duration of the windup.
		while (windupTimer < windupTime)
		{
			windupTimer += Time.deltaTime;

			yield return null;
		}

		//Wait until we've released attack:
		while (input.attack)
		{
			windupTimer += Time.deltaTime;

			windupTimer = Mathf.Clamp(windupTimer, 0f, 2f);

			yield return null;
		}

		windingUp = false;
		inCounterAttack = false;
	}

	//When blocking, then attacking immediately, skip changing stance at least:
	public void WindupInstantly()
	{
		StopAllCoroutines();
		dir = arms.stancePicker.stance;
		inCounterAttack = true;
		//stancePicker.ForceStance(stancePicker.stance);
		StartCoroutine(WindupRoutine(arms.stancePicker.stance));
	}

	protected override void OnUpdate()
	{
		//Initiate the windup, if you're in stance and click attack:
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			if (!windingUp
				&& energyManager.CanSpendStamina(15f)
				//&& !stancePicker.changingStance
				//&& !dodger.dodging
				/*&& !attacker.attacking
				&& !retract.retracting
				&& !stagger.staggering*/
				)
			{
				if (input.attack)
				{
					dir = arms.stancePicker.stance;

					cachedAttack = false;

					StopAllCoroutines();
					StartCoroutine(WindupRoutine(dir));

				}
			}
	}

		//Saving the attack for later if we're in the middle of something:
		if (
			//arms.stancePicker.changingStance
			/*|| */(arms.combatState == WeaponsOfficer.CombatState.Stagger
				&& arms.stagger.staggerTimer > 0.5f)
			|| arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			if (input.attack)
			{
				cachedAttack = true;
			}
		}

		//Released the saved up attack:
		if (cachedAttack
			//&& !arms.stancePicker.changingStance
			&& energyManager.CanSpendStamina(arms.attacker.getStaminaAmount)
			&& arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			dir = arms.stancePicker.stance;
			cachedAttack = false;

			StopAllCoroutines();
			StartCoroutine(WindupRoutine(dir));
		}
	}
}