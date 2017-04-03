﻿using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	[SerializeField] float attackDuration = 0.75f;
	Vector3 inputVec;
	float inputVecMagnitude;
	public WeaponsOfficer.CombatDir dir { get; private set; }
	public bool attacking { get; private set; }
	public float attackStrength { get; private set; }

	[SerializeField] float forwardMoveAmount = 2f;
	[SerializeField] float forwardStickThreshold = 0.4f;

	public delegate void NoParam();
	public event NoParam OnAttackBegin, OnAttackEnd;

	[SerializeField] float staminaAmount = 1.5f;
	public float getStaminaAmount { get { return staminaAmount; } }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		arms.getWeapon.OnCollisionEnterEvent -= OnSwordCollision;
		arms.getWeapon.OnCollisionEnterEvent += OnSwordCollision;

		pilot.move.ProcessVelocity -= TakeStepForward;
		pilot.move.ProcessVelocity += TakeStepForward;
	}

	void TakeStepForward(ref Vector3 velocity)
	{
		if (attacking
			&& pilot.move.inputVec.z > forwardStickThreshold)
		{
			velocity = mech.transform.forward * forwardMoveAmount;
		}
	}

	void OnSwordCollision(Collision col)
	{
		Sword otherSword = col.transform.GetComponent<Sword>();
		BodyPart bodyPart = col.transform.GetComponent<BodyPart>();

		if (otherSword)
		{
			//If I get blocked
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				Stop();
				arms.combatState = WeaponsOfficer.CombatState.Stagger;
				arms.stagger.GetStaggered(dir);
			}
		}
		else if (bodyPart)
		{
			//If I hit someone
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				if (bodyPart.healthManager.takingDamage)
				{
					Stop();
					arms.combatState = WeaponsOfficer.CombatState.Stagger;
					arms.stagger.GetStaggered(dir, 0.8f);
				}
			}
		}
	}

	string AnimFromStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			//case WeaponsOfficer.CombatDir.BottomLeft:
			//	return "Attack Bottom Left";
			//case WeaponsOfficer.CombatDir.BottomRight:
			//	return "Attack Bottom Right";
			case WeaponsOfficer.CombatDir.Top:
				return "Attack Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Attack Top Left";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Attack Top Right";
		}

		return "Windup Top Right";
	}

	public void Stop()
	{
		attacking = false;
		StopAllCoroutines();
	}

	IEnumerator AttackRoutine(WeaponsOfficer.CombatDir dir)
	{
		if (OnAttackBegin != null)
			OnAttackBegin();

		mechSounds.PlaySwordSwingSound();
		attackStrength = windup.windupTimer;
		attackStrength = Mathf.Clamp(attackStrength, 0.5f, 2f);

		energyManager.SpendStamina(staminaAmount * attackStrength);

		attacking = true;
		arms.combatState = WeaponsOfficer.CombatState.Attack;

		float duration = attackDuration;

		animator.CrossFade(AnimFromStance(dir), 0.25f);

		yield return new WaitForSeconds(duration);

		attacking = false;
		arms.combatState = WeaponsOfficer.CombatState.Retract;

		if (OnAttackEnd != null)
			OnAttackEnd();
	}

	public void AttackInstantly(WeaponsOfficer.CombatDir dir)
	{
		StopAllCoroutines();
		StartCoroutine(AttackRoutine(dir));
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Windup)
		{
			if (!arms.windup.windingUp)
			{
				if (!input.attack)
				{
					dir = windup.dir;

					StopAllCoroutines();
					StartCoroutine(AttackRoutine(dir));
				}
			}
		}
	}
}