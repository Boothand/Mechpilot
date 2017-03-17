using System.Collections;
using UnityEngine;

public class BodyPart : Collidable
{
	public enum BodyGroup { Head, Body, Arms, Legs, NumBodyGroups }	
	[SerializeField] BodyGroup bodyGroup;
	public BodyGroup getBodyGroup { get; private set; }
	Rigidbody rbody;



	protected override void OnAwake()
	{
		base.OnAwake();
		rbody = GetComponent<Rigidbody>();
	}

	IEnumerator GetHitBySword(Sword swordHittingMe)
	{
		healthManager.GetHit(bodyGroup, swordHittingMe.swordTipVelocity, swordHittingMe.getSwordTip.position);

		//Play body hit sound

		//rbody.AddForce(swordHittingMe.swordTipVelocity * 50f, ForceMode.Impulse);


		//Play impact animation
		//Vector3 localSwordVelocity = mech.transform.InverseTransformDirection(swordHittingMe.swordTipVelocity);
		//float swordMagnitude = localSwordVelocity.magnitude;

		//float xImpact = /*swordMagnitude **/ -Mathf.Sign(localSwordVelocity.x);
		//xImpact /= 65f;

		//float yImpact = 1f;

		//if (bodyGroup == BodyGroup.Body)
		//	yImpact = 0.37f;
		//if (bodyGroup == BodyGroup.Arms)
		//	yImpact = -0.37f;
		//if (bodyGroup == BodyGroup.Legs)
		//	yImpact = -1f;

		//animator.SetFloat("XImpact", xImpact);
		//animator.SetFloat("YImpact", yImpact);
		//animator.SetTrigger("Impact Hit");

		yield return new WaitForSeconds(1f);

		healthManager.takingDamage = false;
	}

	IEnumerator GetHitByFoot(KickCheck footKickingMe)
	{
		//Play body hit sound
		mechSounds.PlayBodyHitSound(1f);

		float yImpact = 1f;

		if (bodyGroup == BodyGroup.Body)
			yImpact = 0.37f;
		if (bodyGroup == BodyGroup.Arms)
			yImpact = -0.37f;
		if (bodyGroup == BodyGroup.Legs)
			yImpact = -1f;

		float xImpact = Mathf.Sign(transform.localPosition.x);

		animator.SetFloat("XImpact", xImpact);
		animator.SetFloat("YImpact", yImpact);
		animator.SetTrigger("Impact Hit");

		yield return new WaitForSeconds(0.5f);

		healthManager.takingDamage = false;
	}

	void CheckHitBySword(Sword swordHittingMe)
	{
		if (!healthManager.takingDamage && swordHittingMe)
		{
			if (swordHittingMe.arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				healthManager.takingDamage = true;
				StartCoroutine(GetHitBySword(swordHittingMe));
			}
		}
	}

	void CheckHitByFoot(KickCheck footKickingMe)
	{
		if (!healthManager.takingDamage &&
			footKickingMe &&
			footKickingMe.kicker.kicking)
		{
			healthManager.takingDamage = true;
			StartCoroutine(GetHitByFoot(footKickingMe));
			
		}
	}

	

	protected override void OnCollisionEnter(Collision col)
	{
		base.OnCollisionEnter(col);

		if (!IsValid(col.gameObject))
			return;

		Sword swordHittingMe = col.transform.GetComponent<Sword>();
		KickCheck footKickingMe = col.transform.GetComponent<KickCheck>();

		CheckHitBySword(swordHittingMe);
		CheckHitByFoot(footKickingMe);
	}

	void Update()
	{

	}
}