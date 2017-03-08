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
	bool cachedAttack;

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
		if (col.transform.GetComponent<Sword>())
		{
			if (arms.combatState == WeaponsOfficer.CombatState.Attack)
			{
				StopAllCoroutines();
				arms.combatState = WeaponsOfficer.CombatState.Stance;
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
		StopAllCoroutines();
	}

	IEnumerator Attack(WeaponsOfficer.CombatDir dir)
	{
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

		arms.combatState = WeaponsOfficer.CombatState.Retract;
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
					StartCoroutine(Attack(dir));
				}
			}

			//Save the attack for later
			//if (arms.stancePicker.changingStance)
			//{
			//	if (input.attack)
			//	{
			//		cachedAttack = true;
			//	}
			//}

			////Released the saved up attack
			//if (cachedAttack && !arms.stancePicker.changingStance)
			//{
			//	dir = stancePicker.stance;
			//	cachedAttack = false;

			//	StopAllCoroutines();
			//	StartCoroutine(Attack(dir));
			//}
		}
	}
}