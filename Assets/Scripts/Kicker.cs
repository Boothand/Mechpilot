﻿using System.Collections;
using UnityEngine;

public class Kicker : MechComponent
{
	KickCheck kickCheck;
	public bool kicking { get; private set; }
	[SerializeField] float kickDuration = 1f;
	bool hitSomething;
	bool dealingDamage;

	protected override void OnAwake()
	{
		kickCheck = mech.transform.GetComponentInChildren<KickCheck>();
		base.OnAwake();
	}

	void Start()
	{
		kickCheck.OnTriggerEnterEvent -= FootHitSomething;
		kickCheck.OnTriggerEnterEvent += FootHitSomething;
	}

	void FootHitSomething(Collider col)
	{
		if (!kicking)
			return;

		hitSomething = true;

		BodyPart bodypartIKicked = col.transform.GetComponent<BodyPart>();

		if (bodypartIKicked && !dealingDamage)
		{
			//animator.CrossFade("Walk/Crouch", 0.6f);
			float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			animator.CrossFade("Kick Hit 2", 0.1f, 0, 1f - time);

			BodyPart.BodyGroup group = bodypartIKicked.getBodyGroup;
			bodypartIKicked.arms.healthManager.GetHit(group, Vector3.one * 0.06f, kickCheck.transform.position, 10);
			dealingDamage = true;
		}
	}

	IEnumerator KickRoutine()
	{
		kicking = true;
		animator.SetTrigger("Kick");

		yield return new WaitForSeconds(kickDuration);

		if (!hitSomething)
		{
			animator.CrossFade("Walk/Crouch", 0.3f);

			//animator.CrossFade("Kick Miss", 0.35f);
			energyManager.SpendStamina(35f);

		}
		else
		{
			animator.CrossFade("Walk/Crouch", 0.3f);
			energyManager.SpendStamina(25f);
		}


		kicking = false;
		hitSomething = false;
		dealingDamage = false;
	}

	void Update()
	{
		if (input.kick
			&& !kicking
			&& energyManager.CanSpendStamina(25f))
		{
			StartCoroutine(KickRoutine());
		}
	}
}