using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	[SerializeField] float attackDuration = 0.75f;
	Vector3 inputVec;
	float inputVecMagnitude;
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	public Transform targetTransform { get; private set; }
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

	Transform DecideAttackTransform(WeaponsOfficer.CombatDir dir)
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

	public void Stop()
	{
		StopAllCoroutines();
	}

	IEnumerator Attack(WeaponsOfficer.CombatDir dir)
	{
		arms.combatState = WeaponsOfficer.CombatState.Attack;

		targetTransform = DecideAttackTransform(dir);

		Transform rIK = arms.getRhandIKTarget;
		Transform originalTargetTransform = targetTransform;

		while (true)
		{
			Vector3 fromPos = rIK.position;
			Quaternion fromRot = rIK.rotation;
			float attackTimer = 0f;

			float duration = attackDuration;

			//If there are 'keyframes', adjust timing between each so it totals to attackDuration
			if (originalTargetTransform.childCount > 0)
				duration /= originalTargetTransform.childCount + 1;

			float acceleration = 0f;

			while (attackTimer < duration)
			{
				acceleration += Time.deltaTime * 0.5f;
				attackTimer += acceleration;
				rIK.position = Vector3.Lerp(fromPos, targetTransform.position, attackTimer / duration);
				rIK.rotation = Quaternion.Lerp(fromRot, targetTransform.rotation, attackTimer / duration);

				yield return new WaitForEndOfFrame();
			}

			//If the attack 'animation' has 'keyframes'
			if (targetTransform.childCount > 0)
			{
				targetTransform = targetTransform.GetChild(0);
			}
			else
			{
				break;
			}
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
					print("Attack dir: " + dir);

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