using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
{
	//Quaternion targetPosOffset;
	WeaponsOfficer.CombatDir blockStance;		//The direction we block in.
	WeaponsOfficer.CombatDir prevBlockStance;	//The direction we blocked in last frame.
	WeaponsOfficer.CombatDir idealBlock;		//Used during auto-block if enabled.

	//The time it takes to enter the block animation from each state:
	[SerializeField] float blendStance = 0.75f;
	[SerializeField] float blendWindup = 0.75f;
	[SerializeField] float blendBlock = 0.75f;

	//Cannot exit a block before this time has passed:
	[SerializeField] float minBlockTime = 0.2f;
	[SerializeField] float minBlockTimeStagger = 0.5f;	//Special case when staggered.

	[SerializeField] float blockDuration = 0.75f;

	[SerializeField] bool autoBlock;
	public bool blocking { get; private set; }
	bool switchingBlockStance;
	bool holdingBlockButton;
	bool attackInterrupted;

	Coroutine blockRoutine;
	
	public event System.Action OnBlockBegin;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		//Callback from sword when it collides:
		if (arms.getWeapon != null)
			arms.getWeapon.OnCollisionEnterEvent += OnSwordCollision;
	}

	//When swords collide, if the other is attacking and I block it, allow me to counter attack quickly.
	void OnSwordCollision(Collision col)
	{
		Sword otherSword = col.transform.GetComponent<Sword>();

		if (otherSword && otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Attack)
		{
			//If I block the other
			if (arms.combatState == WeaponsOfficer.CombatState.Block
				|| arms.stancePicker.changingStance)
			{
				//Drain some stamina depending on their strength
				energyManager.SpendStamina(15f * otherSword.arms.attacker.attackStrength);
				StartCoroutine(CheckCounterAttackRoutine());
			}
		}
	}

	//If you hit attack within 0.75 seconds, allow winding up instantly and not wait for stance switches etc.
	IEnumerator CheckCounterAttackRoutine()
	{
		float timer = 0f;

		while (timer < 0.75f)
		{
			timer += Time.deltaTime;

			if (input.attack)
			{
				Stop();
				arms.windup.WindupInstantly();
				break;
			}

			yield return null;
		}
	}

	//For cancelling the block switch routine:
	public void Stop()
	{
		StopAllCoroutines();
		blocking = false;
	}

	//Only used for auto block, probably debug only.
	WeaponsOfficer.CombatDir DecideBlockStance(WeaponsOfficer.CombatDir enemyAttackDir)
	{
		switch (enemyAttackDir)
		{
			//case WeaponsOfficer.CombatDir.BottomLeft:
			//	return WeaponsOfficer.CombatDir.BottomRight;
			//case WeaponsOfficer.CombatDir.BottomRight:
			//	return WeaponsOfficer.CombatDir.BottomLeft;
			case WeaponsOfficer.CombatDir.Top:
				return WeaponsOfficer.CombatDir.Top;
			case WeaponsOfficer.CombatDir.TopLeft:
				return WeaponsOfficer.CombatDir.TopRight;
			case WeaponsOfficer.CombatDir.TopRight:
				return WeaponsOfficer.CombatDir.TopLeft;
		}

		return WeaponsOfficer.CombatDir.Top;
	}

	string AnimFromStance(WeaponsOfficer.CombatDir dir, bool alternate = false)
	{
		switch (dir)
		{
			//case WeaponsOfficer.CombatDir.BottomLeft:
			//	return "Block BL";
			//case WeaponsOfficer.CombatDir.BottomRight:
			//	return "Block BR";
			case WeaponsOfficer.CombatDir.Top:
				if (alternate)
					return "Block Top 2";

				return "Block Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Block TL";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Block TR";
		}

		return "Unsupported direction";
	}

	IEnumerator BlockRoutine()
	{
		//Tell other classes
		if (OnBlockBegin != null)
			OnBlockBegin();

		switchingBlockStance = true;
		
		//Check if the feet should play an anim for switching stance:
		pilot.footStanceSwitcher.CheckSwitchStance(prevBlockStance, blockStance);

		//Update the orientation when switching block stance.
		if (blockStance == WeaponsOfficer.CombatDir.TopRight)
			arms.stancePicker.orientation = StancePicker.Orientation.Right;
		else if (blockStance == WeaponsOfficer.CombatDir.TopLeft)
			arms.stancePicker.orientation = StancePicker.Orientation.Left;
		
		//Set the correct blend time to go into the block animation
		float blendTimeToUse = blendStance;

		switch (arms.prevCombatState)
		{
			case WeaponsOfficer.CombatState.Stance:
				blendTimeToUse = blendStance;
				break;
			case WeaponsOfficer.CombatState.Windup:
				blendTimeToUse = blendWindup;
				break;
			case WeaponsOfficer.CombatState.Block:
				blendTimeToUse = blendBlock;
				break;
		}

		//Switch to a more convenient up-block anim if stance was on left side
		bool alternateBlock = false;
		if (prevBlockStance == WeaponsOfficer.CombatDir.TopLeft)
			alternateBlock = true;

		prevBlockStance = blockStance;

		//Go into the block stance animation
		animator.CrossFadeInFixedTime(AnimFromStance(blockStance, alternateBlock), blendTimeToUse);

		//Wait the rest of the duration
		float durationToUse = blockDuration;
		yield return new WaitForSeconds(durationToUse);

		switchingBlockStance = false;

		//Make sure the stance picker knows about the change...
		arms.stancePicker.ForceStance(blockStance);
	}

	//Wait a minimum amount, then wait until you've released block or not currently switching stance:
	IEnumerator BlockTimingRoutine()
	{
		float minBlockTimeToUse = minBlockTime;

		if (arms.prevCombatState == WeaponsOfficer.CombatState.Stagger)
		{
			minBlockTimeToUse = minBlockTimeStagger;
		}

		yield return new WaitForSeconds(minBlockTimeToUse);

		while (input.block
			|| switchingBlockStance)
		{
			yield return null;
		}

		blocking = false;
		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}


	protected override void OnUpdate()
	{
		//Initiate the block
		if (!blocking
			&& input.block
			&& !arms.windup.inCounterAttack)
		{
			blocking = true;

			//Only drain stamina the first time you enter blocking, not when switching after.
			if (!holdingBlockButton)
			{
				energyManager.SpendStamina(10f);
				holdingBlockButton = true;
			}

			//Cancel any other routines from other states.
			arms.stancePicker.Stop();
			arms.windup.Stop();
			arms.attacker.Stop();
			arms.retract.Stop();
			arms.stagger.Stop();
			arms.combatState = WeaponsOfficer.CombatState.Block;

			//Check when we are allowed to stop blocking:
			StartCoroutine(BlockTimingRoutine());

			//Use the stance state's stance,
			blockStance = arms.stancePicker.stance;


			//Enter the block pose, stop the block routine (but not the block timing check routine):
			if (blockRoutine != null)
				StopCoroutine(blockRoutine);

			blockRoutine = StartCoroutine(BlockRoutine());
		}

		if (!input.block)
		{
			holdingBlockButton = false;
		}

		//Every frame while holding block:
		if (arms.combatState == WeaponsOfficer.CombatState.Block)
		{
			//For auto-choosing the direction, probably debug only.
			if (autoBlock
				&& mech.tempEnemy)
			{
				idealBlock = DecideBlockStance(mech.tempEnemy.weaponsOfficer.attacker.attackDir);
				blockStance = idealBlock;
			}
			
			//Update the block stance since the stance picker still chooses directions.
			if (!autoBlock)
			{
				blockStance = arms.stancePicker.stance;
			}

			//Check if the block stance has changed:
			if (prevBlockStance != blockStance)
			{
				//Enter the new block pose:
				if (blockRoutine != null)
				{
					StopCoroutine(blockRoutine);
				}

				blockRoutine = StartCoroutine(BlockRoutine());
			}
		}
	}
}