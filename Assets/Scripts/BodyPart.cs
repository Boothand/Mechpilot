using System.Collections;
using UnityEngine;

//Used for every body part on the puppet,
//checks if the collider gets hit and what to do next.
public class BodyPart : Collidable
{
	//Different behaviour depending on the body group it is.
	public enum BodyGroup { Head, Body, Arms, Legs, NumBodyGroups }	

	[SerializeField] BodyGroup bodyGroup;	//Must be set in inspector
	public BodyGroup getBodyGroup { get; private set; }
	//Rigidbody rbody;

	bool beingHit;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		//Modify velocity before it is applied
		pilot.movement.ProcessVelocity += MoveBackWhenHit;
		dontStopOnDeath = true;
	}

	protected override void OnDead()
	{
		base.OnDead();

		pilot.movement.ProcessVelocity -= MoveBackWhenHit;
	}

	//Move backwards when getting hit.
	public void MoveBackWhenHit(ref Vector3 velocity)
	{
		if (beingHit)
		{
			velocity = -mech.transform.forward * 1.5f;
			//animator.CrossFadeInFixedTime("Step Back From Hit", 0.3f);
		}
	}

	//Maps the impact blend tree in the animator and plays the impact state.
	void SetImpactState(float xImpact, float yImpact)
	{
		if (bodyGroup == BodyGroup.Head)
			yImpact = 1f;

		animator.SetFloat("XImpact", xImpact);
		animator.SetFloat("YImpact", yImpact);

		animator.CrossFadeInFixedTime("Impact Tree", 0.1f, 0);
		animator.CrossFadeInFixedTime("Impact Tree", 0.1f, 2);
	}

	//Takes damage, plays impact animation
	IEnumerator GetHitBySword(Sword swordHittingMe)
	{
		beingHit = true;
		healthManager.GetHit(bodyGroup, swordHittingMe.swordTipVelocity * swordHittingMe.arms.attacker.attackStrength, swordHittingMe.getSwordTip.position);

		//Play impact animation
		Vector3 localSwordVelocity = mech.transform.InverseTransformDirection(swordHittingMe.swordTipVelocity);
		//float swordMagnitude = localSwordVelocity.magnitude;

		float xImpact = /*swordMagnitude **/ -Mathf.Sign(localSwordVelocity.x);
		//xImpact /= 65f;
		float yImpact = 0f;

		SetImpactState(xImpact, yImpact);

		//Stop moving back
		yield return new WaitForSeconds(0.1f);
		beingHit = false;

		yield return new WaitForSeconds(1f);

		healthManager.takingDamage = false;
	}

	IEnumerator GetHitByFootRoutine(KickCheck footKickingMe)
	{
		beingHit = true;
		//A bit different here since it uses a trigger volume.
		//See Kicker.FootHitSomething(), it applies the damage to us instead..

		float xImpact = Mathf.Sign(transform.localPosition.x);
		float yImpact = 1f;

		SetImpactState(xImpact, yImpact);

		yield return new WaitForSeconds(0.5f);

		beingHit = false;
		healthManager.takingDamage = false;
	}

	//Special case because the foot is a trigger..
	public void GetHitByFoot(KickCheck footKickingMe)
	{
		healthManager.takingDamage = true;
		StartCoroutine(GetHitByFootRoutine(footKickingMe));

	}

	protected override void RunCollisionEvent(Collision col)
	{
		base.RunCollisionEvent(col);

		if (!IsValid(col.gameObject))
			return;

		Sword swordHittingMe = col.transform.GetComponent<Sword>();
		KickCheck footKickingMe = col.transform.GetComponent<KickCheck>();

		//Check if a sword is hitting us
		if (swordHittingMe &&
			swordHittingMe.arms.combatState == WeaponsOfficer.CombatState.Attack &&
			!healthManager.takingDamage)
		{
			healthManager.takingDamage = true;
			StartCoroutine(GetHitBySword(swordHittingMe));
		}

		//Check if a foot is kicking us - NOTE: THIS NEVER RUNS..
		/*
		if (footKickingMe &&
			footKickingMe.kicker.kicking &&
			!healthManager.takingDamage)
		{
			healthManager.takingDamage = true;
			StartCoroutine(GetHitByFoot(footKickingMe));
		}
		*/
	}
}