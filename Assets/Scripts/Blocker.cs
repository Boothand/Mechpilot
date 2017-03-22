﻿using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
{
	//Quaternion targetPosOffset;
	public Quaternion targetRotOffset;
	WeaponsOfficer.CombatDir blockStance;
	WeaponsOfficer.CombatDir idealBlock;
	WeaponsOfficer.CombatDir prevBlockStance;

	[SerializeField] float minBlockTime = 0.5f;

	[SerializeField] float blockDuration = 0.75f;

	[SerializeField] bool autoBlock;
	public bool blocking { get; private set; }
	bool switchingBlockStance;

	public Mech tempEnemy;
	Coroutine blockRoutine;

	public delegate void NoParam();
	public event NoParam OnBlockBegin;

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
		Sword otherSword = col.transform.GetComponent<Sword>();
		if (otherSword && otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Attack)
		{
			//If I block the other
			if (arms.combatState == WeaponsOfficer.CombatState.Block
				|| stancePicker.changingStance)
			{
				energyManager.SpendStamina(15f * otherSword.attacker.attackStrength);
				StartCoroutine(CheckCounterAttackRoutine());
			}
		}
	}

	IEnumerator CheckCounterAttackRoutine()
	{
		float timer = 0f;

		while (timer < 0.5f)
		{
			timer += Time.deltaTime;

			if (input.attack)
			{
				Stop();
				windup.WindupInstantly();
				break;
			}

			yield return null;
		}
	}

	public void Stop()
	{
		StopAllCoroutines();
		blocking = false;
	}

	WeaponsOfficer.CombatDir DecideBlockStance(WeaponsOfficer.CombatDir enemyAttackDir)
	{
		switch (enemyAttackDir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return WeaponsOfficer.CombatDir.BottomRight;
			case WeaponsOfficer.CombatDir.BottomRight:
				return WeaponsOfficer.CombatDir.BottomLeft;
			case WeaponsOfficer.CombatDir.Top:
				return WeaponsOfficer.CombatDir.Top;
			case WeaponsOfficer.CombatDir.TopLeft:
				return WeaponsOfficer.CombatDir.TopRight;
			case WeaponsOfficer.CombatDir.TopRight:
				return WeaponsOfficer.CombatDir.TopLeft;
		}

		return WeaponsOfficer.CombatDir.Top;
	}

	string AnimFromStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return "Block BL";
			case WeaponsOfficer.CombatDir.BottomRight:
				return "Block BR";
			case WeaponsOfficer.CombatDir.Top:
				return "Block Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Block TL";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Block TR";
		}

		return "Block Top";
	}

	IEnumerator BlockRoutine()
	{
		if (OnBlockBegin != null)
			OnBlockBegin();

		switchingBlockStance = true;

		prevBlockStance = blockStance;
		float durationToUse = blockDuration;

		//float timer = 0f;

		animator.CrossFade(AnimFromStance(blockStance), 0.5f);

		yield return new WaitForSeconds(durationToUse);

		//while (timer < durationToUse)
		//{
		//	timer += Time.deltaTime;

		//	//arms.InterpolateIKPose(targetPose, timer / durationToUse);
		//	yield return null;
		//}

		switchingBlockStance = false;
		stancePicker.ForceStance(blockStance);
	}

	IEnumerator BlockTimingRoutine()
	{
		yield return new WaitForSeconds(minBlockTime);

		while (input.block
			|| switchingBlockStance)
		{
			yield return null;
		}

		blocking = false;
		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}

	void Update()
	{
		//Check if prev block state == block state.
		//Start changing block stance:
			//Check if it's necessary to go via another stance.
			//Do so in half the time

		//Initiate the block
		if (!blocking && input.block)
		{
			blocking = true;

			stancePicker.Stop();
			windup.Stop();
			attacker.Stop();
			retract.Stop();
			stagger.Stop();
			arms.combatState = WeaponsOfficer.CombatState.Block;

			//Check when we are allowed to stop blocking:
			StartCoroutine(BlockTimingRoutine());

			blockStance = stancePicker.stance;
			//Enter the block pose:
			if (blockRoutine != null)
			{
				StopCoroutine(blockRoutine);
			}

			blockRoutine = StartCoroutine(BlockRoutine());
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Block)
		{
			if (autoBlock)
			{
				idealBlock = DecideBlockStance(tempEnemy.weaponsOfficer.attacker.dir);
				blockStance = idealBlock;
			}
			else
			{
				blockStance = stancePicker.stance;
			}

			if (prevBlockStance != blockStance)
			{
				//Enter the block pose:
				if (blockRoutine != null)
				{
					StopCoroutine(blockRoutine);
				}

				blockRoutine = StartCoroutine(BlockRoutine());
			}

			//if (!switchingBlockStance)
			//{
			//	arms.StoreTargets();
			//	arms.InterpolateIKPose2(targetPose, targetRotOffset, Time.deltaTime * 4f);
			//}

			//targetPose = GetTargetPose(blockStance);

			//AdjustPosition();

			//Only for the sake of maintaining crouch height atm

		}
	}
}