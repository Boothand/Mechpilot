﻿using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	[SerializeField] float attackDuration = 0.75f;
	Vector3 inputVec;
	float inputVecMagnitude;
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;
	public IKPose targetPose { get; private set; }
	public WeaponsOfficer.CombatDir dir { get; private set; }
	bool cachedAttack;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		arms.getWeapon.OnCollision -= OnSwordCollision;
		arms.getWeapon.OnCollision += OnSwordCollision;
	}

	void OnSwordCollision(Collision col)
	{
		if (col.transform.GetComponent<Sword>())
		{
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				StopAllCoroutines();
				arms.combatState = WeaponsOfficer.CombatState.Stance;
				//Stagger?
			}
		}
	}

	IKPose DecideAttackTransform(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
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
	}

	IEnumerator Attack(WeaponsOfficer.CombatDir dir)
	{
		arms.combatState = WeaponsOfficer.CombatState.Attack;
		targetPose = DecideAttackTransform(dir);

		bool keyframeAnim = false;
		int keyFrames = targetPose.rHand.childCount;


		if (keyFrames > 0)
			keyframeAnim = true;

		while (true)
		{
			arms.StoreTargets();
			float attackTimer = 0f;

			float duration = attackDuration;

			//If there are 'keyframes', adjust timing between each so it totals to attackDuration
			if (keyframeAnim)
				duration /= keyFrames + 1;

			print(duration);

			float acceleration = 0f;

			while (attackTimer < duration)
			{
				acceleration += Time.deltaTime * 0.5f;
				attackTimer += acceleration;
				arms.InterpolateIKPose(targetPose, attackTimer / duration);

				yield return null;
			}

			//If the attack 'animation' has 'keyframes'
			if (targetPose.rHand.childCount > 0)
			{
				targetPose.rHand = targetPose.rHand.GetChild(0);
			}
			else
			{
				break;
			}
		}
		
		if (keyframeAnim)
		{
			for (int i = 0; i < keyFrames; i++)
			{
				targetPose.rHand = targetPose.rHand.parent;
			}
		}

		arms.combatState = WeaponsOfficer.CombatState.Retract;
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
					StartCoroutine(Attack(dir));
				}
			}

			//Save the attack for later
			//if (arms.stancePicker.changingStance)
			//{
			//	if (input.attack)
			//	{
			//		cachedAttack = true;
			//	}
			//}

			////Released the saved up attack
			//if (cachedAttack && !arms.stancePicker.changingStance)
			//{
			//	dir = stancePicker.stance;
			//	cachedAttack = false;

			//	StopAllCoroutines();
			//	StartCoroutine(Attack(dir));
			//}
		}
	}
}