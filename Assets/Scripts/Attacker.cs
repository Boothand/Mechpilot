using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	[SerializeField] float attackDuration = 0.75f;
	[SerializeField] float blendTime = 0.1f;
	[SerializeField] float blendTimeFeet = 0.25f;
	Vector3 inputVec;
	float inputVecMagnitude;
	public WeaponsOfficer.CombatDir dir { get; private set; }
	public bool attacking { get; private set; }
	public float attackStrength { get; private set; }
	bool canTakeForwardStep;

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
				if (moveDir.z > 0.4f)
				{
					//arms.TweenLayerWeight(0f, 1, 0.1f);
					animator.CrossFadeInFixedTime("Attack TL Step", blendTimeFeet, 0);
					return "Attack TL Step";
				}
				return "Attack Top Left";
			case WeaponsOfficer.CombatDir.TopRight:
				if (moveDir.z > 0.4f)
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
		attacking = false;
		StopAllCoroutines();
	}

	IEnumerator AttackRoutine(WeaponsOfficer.CombatDir dir)
	{
		if (OnAttackBegin != null)
			OnAttackBegin();

		canTakeForwardStep = true;

		mechSounds.PlaySwordSwingSound();
		attackStrength = windup.windupTimer;
		attackStrength = Mathf.Clamp(attackStrength, 0.5f, 2f);

		energyManager.SpendStamina(staminaAmount * attackStrength);

		attacking = true;
		arms.combatState = WeaponsOfficer.CombatState.Attack;

		float duration = attackDuration;

		animator.CrossFadeInFixedTime(AnimFromStance(dir, pilot.move.inputVec), blendTime, 1);


		yield return new WaitForSeconds(0.1f);
		canTakeForwardStep = false;
		yield return new WaitForSeconds(duration - 0.1f);

		attacking = false;
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
					dir = windup.dir;

					StopAllCoroutines();
					StartCoroutine(AttackRoutine(dir));
				}
			}
		}
	}
}