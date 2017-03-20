using System.Collections;
using UnityEngine;

public class Windup : MechComponent
{
	[SerializeField] float windupTime = 0.5f;
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;
	public IKPose targetTransform { get; private set; }
	public bool windingUp { get; private set; }
	public WeaponsOfficer.CombatDir dir { get; private set; }
	bool cachedAttack;
	public float windupTimer { get; private set; }
	public delegate void NoParam();
	public event NoParam OnWindupBegin;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	string AnimFromStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return "Windup Bottom Left";
			case WeaponsOfficer.CombatDir.BottomRight:
				return "Windup Bottom Right";
			case WeaponsOfficer.CombatDir.Top:
				return "Windup Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Windup Top Left";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Windup Top Right";
		}

		return "Windup Top Right";
	}

	IKPose DecideWindupTransform()
	{
		switch (stancePicker.stance)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return blTransform;

			case WeaponsOfficer.CombatDir.BottomRight:
				return brTransform;

			case WeaponsOfficer.CombatDir.Top:
				return topTransform;

			case WeaponsOfficer.CombatDir.TopLeft:
				return tlTransform;

			case WeaponsOfficer.CombatDir.TopRight:
				return trTransform;
		}

		return trTransform;
	}

	public void Stop()
	{
		StopAllCoroutines();
		windingUp = false;
		cachedAttack = false;
	}

	IEnumerator WindupRoutine(WeaponsOfficer.CombatDir dir)
	{
		if (OnWindupBegin != null)
			OnWindupBegin();

		windingUp = true;
		arms.combatState = WeaponsOfficer.CombatState.Windup;

		targetTransform = DecideWindupTransform();

		float timer = 0f;

		arms.StoreTargets();

		windupTimer = 0f;

		animator.CrossFade(AnimFromStance(stancePicker.stance), windupTime);

		while (timer < windupTime)
		{
			timer += Time.deltaTime;

			windupTimer += Time.deltaTime;

			arms.InterpolateIKPose(targetTransform, timer / windupTime);

			yield return null;
		}

		while (input.attack)
		{
			windupTimer += Time.deltaTime;

			windupTimer = Mathf.Clamp(windupTimer, 0f, 2f);

			yield return null;
		}

		windingUp = false;
	}

	public void WindupInstantly()
	{
		StopAllCoroutines();
		dir = stancePicker.stance;
		//stancePicker.ForceStance(stancePicker.stance);
		StartCoroutine(WindupRoutine(stancePicker.stance));
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			if (!windingUp
				&& energyManager.CanSpendStamina(15f)
				&& !stancePicker.changingStance
				&& !dodger.dodging
				&& !attacker.attacking
				&& !retract.retracting
				&& !stagger.staggering
				)
			{
				if (input.attack)
				{
					dir = stancePicker.stance;

					cachedAttack = false;

					StopAllCoroutines();
					StartCoroutine(WindupRoutine(dir));

				}
			}

		//print(windingUp);
	}

		//Save the attack for later
		if (arms.stancePicker.changingStance
			|| (arms.combatState == WeaponsOfficer.CombatState.Stagger
				&& arms.stagger.staggerTimer > 0.5f)
			|| arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			if (input.attack)
			{
				cachedAttack = true;
			}
		}

		////Released the saved up attack
		if (cachedAttack
			&& !arms.stancePicker.changingStance
			&& energyManager.CanSpendStamina(attacker.getStaminaAmount)
			&& arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			dir = stancePicker.stance;
			cachedAttack = false;

			StopAllCoroutines();
			StartCoroutine(WindupRoutine(dir));
		}
	}
}