using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	[SerializeField] float attackDuration = 0.75f;
	Vector3 inputVec;
	float inputVecMagnitude;
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;
	[SerializeField] IKPose trTransform2, tlTransform2, brTransform2, blTransform2, topTransform2;
	public IKPose targetPose { get; private set; }
	public WeaponsOfficer.CombatDir dir { get; private set; }
	public bool attacking { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		arms.getWeapon.OnCollision -= OnSwordCollision;
		arms.getWeapon.OnCollision += OnSwordCollision;
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
				//stancePicker.StartCoroutine(stancePicker.ForceStanceRoutine());
				//Stagger?
			}
		}
		else if (bodyPart)
		{
			//If I hit someone
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				Stop();
				arms.combatState = WeaponsOfficer.CombatState.Stagger;
				arms.stagger.GetStaggered(dir, 0.8f);
				//stancePicker.StartCoroutine(stancePicker.ForceStanceRoutine());
				//Stagger?
			}
		}
	}

	IKPose DecideAttackTransform(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return blTransform;

			case WeaponsOfficer.CombatDir.BottomRight:
				return brTransform;

			case WeaponsOfficer.CombatDir.Top:
				return topTransform;

			case WeaponsOfficer.CombatDir.TopLeft:
				return tlTransform;

			case WeaponsOfficer.CombatDir.TopRight:
				return trTransform;
		}

		return trTransform;
	}

	IKPose DecideAttackTransform2(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return blTransform2;

			case WeaponsOfficer.CombatDir.BottomRight:
				return brTransform2;

			case WeaponsOfficer.CombatDir.Top:
				return topTransform2;

			case WeaponsOfficer.CombatDir.TopLeft:
				return tlTransform2;

			case WeaponsOfficer.CombatDir.TopRight:
				return trTransform2;
		}

		return trTransform2;
	}

	public void Stop()
	{
		attacking = false;
		StopAllCoroutines();
	}

	IEnumerator AttackRoutine(WeaponsOfficer.CombatDir dir)
	{
		attacking = true;
		arms.combatState = WeaponsOfficer.CombatState.Attack;
		targetPose = DecideAttackTransform(dir);
		IKPose finalPose = DecideAttackTransform2(dir);
		
		arms.StoreTargets();
		float attackTimer = 0f;

		float duration = attackDuration;			

		float acceleration = 0f;

		while (attackTimer < duration)
		{
			acceleration += Time.deltaTime * 0.5f;
			attackTimer += acceleration;
			arms.InterpolateIKPose(targetPose, attackTimer / duration);

			yield return null;
		}

		attackTimer = 0f;
		arms.StoreTargets();

		while (attackTimer < duration)
		{
			acceleration += Time.deltaTime * 0.5f;
			attackTimer += acceleration;
			arms.InterpolateIKPose(finalPose, attackTimer / duration);

			yield return null;
		}

		attacking = false;
		arms.combatState = WeaponsOfficer.CombatState.Retract;
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