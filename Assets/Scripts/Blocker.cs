using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
{
	[SerializeField] float armHeight, armDistance = 0.5f;
	[SerializeField] Transform chest;
	[SerializeField] Vector3 rotOffset;
	[SerializeField] float swipeDuration = 0.75f;
	bool inBlockSwipe;
	Vector3 idlePos;
	Quaternion idleRot;
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	Transform targetTransform;
	WeaponsOfficer.CombatDir blockStance;

	public Mech tempEnemy;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	Transform GetTargetTransform(WeaponsOfficer.CombatDir dir)
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

		return topTransform;
	}

	WeaponsOfficer.CombatDir DecideBlockStance(WeaponsOfficer.CombatDir enemyAttackDir)
	{
		switch (enemyAttackDir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return WeaponsOfficer.CombatDir.BottomRight;
			case WeaponsOfficer.CombatDir.BottomRight:
				return WeaponsOfficer.CombatDir.BottomLeft;
			case WeaponsOfficer.CombatDir.Top:
				return WeaponsOfficer.CombatDir.Top;
			case WeaponsOfficer.CombatDir.TopLeft:
				return WeaponsOfficer.CombatDir.TopRight;
			case WeaponsOfficer.CombatDir.TopRight:
				return WeaponsOfficer.CombatDir.TopLeft;
		}

		return WeaponsOfficer.CombatDir.Top;
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Block)
		{
			blockStance = DecideBlockStance(tempEnemy.weaponsOfficer.stancePicker.stance);

			targetTransform = GetTargetTransform(blockStance);

			Transform rIK = arms.armControl.getRhandIKTarget;

			rIK.position = Vector3.Lerp(rIK.position, targetTransform.position, Time.deltaTime * 4f);
			rIK.rotation = Quaternion.Lerp(rIK.rotation, targetTransform.rotation, Time.deltaTime * 4f);
		}
	}
}