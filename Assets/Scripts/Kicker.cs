using System.Collections;
using UnityEngine;

public class Kicker : MechComponent
{
	KickCheck kickCheck;	//The class for the trigger volume on our foot
	public bool kicking { get; private set; }

	[SerializeField] float kickDuration = 1f;

	//First drain base usage from stamina, then one of the two, either hit or miss usage.
	[SerializeField] float staminaBaseUsage = 10f;
	[SerializeField] float staminaHitUsage = 5f;
	[SerializeField] float staminaMissUsage = 15f;

	bool hitSomething;
	bool dealingDamage;

	protected override void OnAwake()
	{
		kickCheck = mech.transform.GetComponentInChildren<KickCheck>();
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		//Callback when the foot trigger detects something valid.
		kickCheck.OnTriggerEnterEvent += FootHitSomething;
		pilot.movement.ProcessVelocity += SlowDownOnKick;
	}

	void SlowDownOnKick(ref Vector3 velocity)
	{
		if (kicking)
		{
			velocity *= Time.deltaTime * 50f;
		}
	}

	//If we hit someone's bodypart during the kick sequence,
	//play an animation on ourselves and apply damage.
	void FootHitSomething(Collider col)
	{
		if (!kicking)
			return;

		hitSomething = true;

		BodyPart bodypartIKicked = col.transform.GetComponent<BodyPart>();

		if (bodypartIKicked && !dealingDamage)
		{
			//Just play the kick anim reversed from where we were...
			float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			animator.CrossFade("Kick Hit 2", 0.1f, 0, 1f - time);
			
			BodyPart.BodyGroup group = bodypartIKicked.getBodyGroup;
			bodypartIKicked.arms.healthManager.GetHit(group, Vector3.one * 0.06f, kickCheck.transform.position, 10);
			dealingDamage = true;

			bodypartIKicked.GetHitByFoot(kickCheck);
		}
	}
	
	IEnumerator KickRoutine()
	{
		kicking = true;

		//Play the kick animation
		animator.CrossFadeInFixedTime("Kick", 0.25f);

		energyManager.SpendStamina(staminaBaseUsage);

		//Wait until the kick is done.
		yield return new WaitForSeconds(kickDuration);


		//Drain different stamina if we hit or not.
		if (!hitSomething)
		{
			//animator.CrossFade("Kick Miss", 0.35f);
			energyManager.SpendStamina(staminaMissUsage);
		}
		else
		{
			energyManager.SpendStamina(staminaHitUsage);
		}

		//Tween back to stance anim
		animator.CrossFadeInFixedTime(arms.stancePicker.OrientationAnim(), 0.3f);

		kicking = false;
		hitSomething = false;
		dealingDamage = false;
	}

	//If we press kick and not already kicking:
	protected override void OnUpdate()
	{
		if (input.kick
			&& !kicking
			&& energyManager.CanSpendStamina(staminaBaseUsage + staminaHitUsage))
		{
			StartCoroutine(KickRoutine());
		}
	}
}