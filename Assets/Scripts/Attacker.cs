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

			while (attackTimer < duration)
			{
				attackTimer += Time.deltaTime;
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