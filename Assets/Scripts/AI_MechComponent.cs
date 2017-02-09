//using System.Collections;
using UnityEngine;

public class AI_MechComponent : MechComponent
{
	protected Mech enemy;
	protected AI_Combat aiCombat;
	protected AI_Movement aiMovement;
	protected AI_Attacker aiAttacker;
	protected float enemyFrontDot, enemyRightDot;
	protected Vector3 dirToEnemy;
	protected float distanceToEnemy;

	protected override void OnAwake()
	{
		base.OnAwake();

		Mech[] mechs = FindObjectsOfType<Mech>();
		
		for (int i = 0; i < mechs.Length; i++)
		{
			if (mechs[i] != mech)
			{
				enemy = mechs[i];
				break;
			}
		}

		aiCombat = transform.root.GetComponentInChildren<AI_Combat>();
		aiMovement = transform.root.GetComponentInChildren<AI_Movement>();
		aiAttacker = transform.root.GetComponentInChildren<AI_Attacker>();
	}

	protected bool IsWithinCombatDistance()
	{
		return distanceToEnemy < aiCombat.getCombatDistance;
	}

	protected bool FacingTarget(Vector3 target)
	{
		if (enemyFrontDot > 0.95f)
		{
			return true;
		}

		return false;
	}

	protected bool CanSwingAtEnemy(Transform target)
	{
		return IsWithinCombatDistance() && FacingTarget(target.position);
	}

	protected virtual void Update()
	{
		Vector3 dirToEnemy = (enemy.transform.position - mech.transform.position).normalized;
		enemyFrontDot = Vector3.Dot(mech.transform.forward, dirToEnemy);
		enemyRightDot = Vector3.Dot(mech.transform.right, dirToEnemy);
		dirToEnemy = (enemy.transform.position - mech.transform.position).normalized;
		distanceToEnemy = (enemy.transform.position - mech.transform.position).magnitude;
	}
}