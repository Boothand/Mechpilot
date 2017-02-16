//using System.Collections;
using UnityEngine;

public class BodyPart : Collidable
{
	public enum BodyGroup { Head, Body, Arms, Legs, NumBodyGroups }	
	[SerializeField] BodyGroup bodyGroup;

	public BodyGroup getBodyGroup { get; private set; }



	protected override void OnAwake()
	{
		base.OnAwake();
	}
	

	protected override void OnTriggerEnter(Collider col)
	{
		base.OnTriggerEnter(col);

		if (!IsValid(col.gameObject))
			return;

		Sword swordHittingMe = col.GetComponent<Sword>();
		KickCheck footKickingMe = col.GetComponent<KickCheck>();

		if (swordHittingMe)
		{
			if (swordHittingMe.arms.armControl.prevState == ArmControl.State.Attack)
			{
				healthManager.GetHit(bodyGroup, swordHittingMe.swordTipVelocity);

				//Play body hit sound
				mechSounds.PlayBodyHitSound(1f);

				//Play impact animation
				Vector3 localSwordVelocity = mech.transform.InverseTransformDirection(swordHittingMe.swordTipVelocity);
				float swordMagnitude = localSwordVelocity.magnitude;

				float xImpact = /*swordMagnitude **/ -Mathf.Sign(localSwordVelocity.x);
				xImpact /= 65f;

				float yImpact = 1f;

				if (bodyGroup == BodyGroup.Body)
					yImpact = 0.37f;
				if (bodyGroup == BodyGroup.Arms)
					yImpact = -0.37f;
				if (bodyGroup == BodyGroup.Legs)
					yImpact = -1f;

				animator.SetFloat("XImpact", xImpact);
				animator.SetFloat("YImpact", yImpact);
				animator.SetTrigger("Impact Hit");
			}
		}

		if (footKickingMe && footKickingMe.kicker.kicking)
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
		}
	}

	void Update()
	{

	}
}