using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	//How long to wait until entering the retract state:
	[SerializeField] float attackDuration = 0.75f;

	//Time in seconds to blend into the attack animation on the torso:
	[SerializeField] float blendTime = 0.1f;

	//Time in seconds to blend into the attack animation on the feet:
	[SerializeField] float blendTimeFeet = 0.25f;

	//How much stamina to drain from an attack
	[SerializeField] float staminaAmount = 1.5f;

	//The direction to attack:
	public WeaponsOfficer.CombatDir attackDir { get; private set; }

	//Influences stamina drain from you and the one who blocks, and damage taken:
	public float attackStrength { get; private set; }	

	//Decides if the character should step forward with an attack or not
	bool canTakeForwardStep;

	//How far to leap forward when doing the forward step:
	[SerializeField] float forwardMoveAmount = 2f;

	//Ignore any input less than this value when evaluating the forward step:
	[SerializeField] float forwardStickThreshold = 0.4f;
	
	//Events free for other classes to make use of.
	public System.Action OnAttackBegin, OnAttackEnd;

	public float getStaminaAmount { get { return staminaAmount; } }



	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		if (arms.getWeapon != null)
		{
			arms.getWeapon.OnCollisionEnterEvent -= OnSwordCollision;
			arms.getWeapon.OnCollisionEnterEvent += OnSwordCollision;
		}

		pilot.move.ProcessVelocity -= TakeStepForward;
		pilot.move.ProcessVelocity += TakeStepForward;
	}

	void TakeStepForward(ref Vector3 velocity)
	{
		if (canTakeForwardStep
			&& pilot.move.inputVec.z > forwardStickThreshold)
		{
			velocity = mech.transform.forward * forwardMoveAmount;
		}
	}

	void OnSwordCollision(Collision col)
	{
		Sword otherSword = col.transform.GetComponent<Sword>();
		BodyPart otherBodyPart = col.transform.GetComponent<BodyPart>();		

		if (otherSword)
		{
			//If I get blocked
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				Stop();
				arms.combatState = WeaponsOfficer.CombatState.Stagger;
				arms.stagger.GetStaggered(attackDir);
			}
		}
		else if (otherBodyPart)
		{
			//If I hit someone
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				if (otherBodyPart.healthManager.takingDamage)
				{
					Stop();
					arms.combatState = WeaponsOfficer.CombatState.Stagger;
					arms.stagger.GetStaggered(attackDir, 0.8f);
				}
			}
		}


	}

	string AnimFromStance(WeaponsOfficer.CombatDir dir, Vector3 moveDir)
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
				if (moveDir.z > 0.4f
					&& !pilot.croucher.crouching)
				{
					//arms.TweenLayerWeight(0f, 1, 0.1f);
					animator.CrossFadeInFixedTime("Attack TL Step", blendTimeFeet, 0);
					return "Attack TL Step";
				}
				return "Attack Top Left";
			case WeaponsOfficer.CombatDir.TopRight:
				if (moveDir.z > 0.4f
					&& !pilot.croucher.crouching)
				{
					//arms.TweenLayerWeight(0f, 1, 0.1f);
					animator.CrossFadeInFixedTime("Attack TR Step", blendTimeFeet, 0);
					return "Attack TR Step";
				}
				return "Attack Top Right";
		}

		return "Windup Top Right";
	}

	public void Stop()
	{
		StopAllCoroutines();
	}

	IEnumerator AttackRoutine(WeaponsOfficer.CombatDir dir)
	{
		if (OnAttackBegin != null)
			OnAttackBegin();

		canTakeForwardStep = true;

		attackStrength = arms.windup.windupTimer;
		attackStrength = Mathf.Clamp(attackStrength, 0.5f, 2f);

		energyManager.SpendStamina(staminaAmount * attackStrength);
		
		arms.combatState = WeaponsOfficer.CombatState.Attack;

		float duration = attackDuration;

		animator.CrossFadeInFixedTime(AnimFromStance(dir, pilot.move.inputVec), blendTime, 1);


		yield return new WaitForSeconds(0.1f);

		canTakeForwardStep = false;

		yield return new WaitForSeconds(0.1f);
		mechSounds.PlaySwordSwingSound();

		yield return new WaitForSeconds(duration - 0.2f);
		
		arms.combatState = WeaponsOfficer.CombatState.Retract;
		//arms.TweenLayerWeight(1f, 1, 0.3f);

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
					attackDir = arms.windup.dir;

					StopAllCoroutines();
					StartCoroutine(AttackRoutine(attackDir));
				}
			}
		}
	}
}