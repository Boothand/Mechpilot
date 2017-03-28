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
			BodyPart.BodyGroup group = bodypartIKicked.getBodyGroup;
			bodypartIKicked.arms.healthManager.GetHit(group, Vector3.one * 0.06f, kickCheck.transform.position, 10);
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
			energyManager.SpendStamina(20f);

		}
		else
		{
			animator.CrossFade("Kick Hit", 0.25f);
			energyManager.SpendStamina(10f);
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