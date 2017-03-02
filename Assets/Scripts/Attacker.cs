using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	[SerializeField] float attackDuration = 0.75f;
	Vector3 inputVec;
	float inputVecMagnitude;
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	Transform targetTransform;
	WeaponsOfficer.CombatDir dir;

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
				//Stagger?
			}
		}
	}

	Transform DecideAttackTransform()
	{
		switch (stancePicker.stance)
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

	IEnumerator Attack(WeaponsOfficer.CombatDir dir)
	{
		targetTransform = DecideAttackTransform();

		Transform rIK = arms.armControl.getRhandIKTarget;
		

		while (true)
		{
			Vector3 fromPos = rIK.position;
			Quaternion fromRot = rIK.rotation;
			float attackTimer = 0f;

			float duration = attackDuration;

			//If there are 'keyframes', adjust timing between each so it totals to attackDuration
			if (targetTransform.childCount > 0)
				duration /= targetTransform.childCount + 1;

			while (attackTimer < attackDuration)
			{
				print(targetTransform.name);
				attackTimer += Time.deltaTime;
				rIK.position = Vector3.Lerp(fromPos, targetTransform.position, attackTimer / attackDuration);
				rIK.rotation = Quaternion.Lerp(fromRot, targetTransform.rotation, attackTimer / attackDuration);

				yield return null;
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

		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			if (input.attack)
			{
				arms.combatState = WeaponsOfficer.CombatState.Attack;
				dir = stancePicker.stance;

				StopAllCoroutines();
				StartCoroutine(Attack(dir));
			}
		}
	}
}