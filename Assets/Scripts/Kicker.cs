using System.Collections;
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
		kickCheck.OnCollision -= FootHitSomething;
		kickCheck.OnCollision += FootHitSomething;
	}

	void FootHitSomething(Collision col)
	{
		hitSomething = true;

		BodyPart bodypartIKicked = col.transform.GetComponent<BodyPart>();

		if (bodypartIKicked && !dealingDamage)
		{
			BodyPart.BodyGroup group = bodypartIKicked.getBodyGroup;
			bodypartIKicked.arms.healthManager.GetHit(group, Vector3.one * 10f);
			dealingDamage = true;
		}
	}

	IEnumerator KickRoutine()
	{
		animator.SetTrigger("Kick");

		yield return new WaitForSeconds(kickDuration);

		if (!hitSomething)
		{
			animator.CrossFade("Kick Miss", 0.35f);
		}
		else
		{
			animator.CrossFade("Kick Hit", 0.25f);
		}

		kicking = false;
		hitSomething = false;
		dealingDamage = false;
	}

	void Update()
	{
		if (input.kick && !kicking)
		{
			kicking = true;
			StartCoroutine(KickRoutine());
		}
	}
}