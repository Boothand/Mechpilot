using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
{
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	Transform targetTransform;
	Vector3 targetPosOffset;
	Quaternion targetRotOffset;
	WeaponsOfficer.CombatDir blockStance;

	[SerializeField] bool autoBlock = true;
	public bool blocking { get; private set; }

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

	void AdjustPosition()
	{
		Transform rIK = arms.getRhandIKTarget;

		Vector3 myMidPoint = arms.getWeapon.getMidPoint.position;
		Vector3 otherMidPoint = tempEnemy.weaponsOfficer.getWeapon.getMidPoint.position;

		
		if (tempEnemy.weaponsOfficer.combatState == WeaponsOfficer.CombatState.Attack)
		{
			//Up/down
			if (myMidPoint.y < otherMidPoint.y)
			{
				targetPosOffset += Vector3.up * Time.deltaTime * 2f;
			}
			else if (myMidPoint.y > otherMidPoint.y)
			{
				targetPosOffset -= Vector3.up * Time.deltaTime * 2f;
			}

			Vector3 localMyPoint = mech.transform.InverseTransformPoint(myMidPoint);
			Vector3 localOtherPoint = mech.transform.InverseTransformPoint(otherMidPoint);
			//tempEnemy.weaponsOfficer.getWeapon.getSwordTip

			//Left/right
			if (localMyPoint.x < localOtherPoint.x)
			{
				targetPosOffset += mech.transform.right * Time.deltaTime * 2f;
				//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(50f, 0, 0f);
			}
			else if (localMyPoint.x > localOtherPoint.x)
			{
				targetPosOffset -= mech.transform.right * Time.deltaTime * 2f;
				//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(-50f, 0f, 0f);
			}

			//Forward/back
			if (localMyPoint.z < localOtherPoint.z)
			{
				targetPosOffset += mech.transform.forward * Time.deltaTime * 2f;
				//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(50f, 0, 0f);
			}
			else if (localMyPoint.z > localOtherPoint.z)
			{
				targetPosOffset -= mech.transform.forward * Time.deltaTime * 2f;
				//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(-50f, 0f, 0f);
			}
		}
		else
		{
			targetPosOffset = Vector3.Lerp(targetPosOffset, Vector3.zero, Time.deltaTime * 3f);
			targetRotOffset = Quaternion.Lerp(targetRotOffset, Quaternion.identity, Time.deltaTime * 3f);
		}
	}

	void Update()
	{
		blocking = false;

		if (arms.combatState == WeaponsOfficer.CombatState.Block)
		{
			blocking = true;

			if (autoBlock)
			{
				blockStance = DecideBlockStance(tempEnemy.weaponsOfficer.stancePicker.stance);
			}
			else
			{
				blockStance = stancePicker.stance;
			}

			targetTransform = GetTargetTransform(blockStance);

			Transform rIK = arms.getRhandIKTarget;

			AdjustPosition();

			rIK.position = Vector3.Lerp(rIK.position, targetTransform.position + targetPosOffset, Time.deltaTime * 4f);
			rIK.rotation = Quaternion.Lerp(rIK.rotation, targetTransform.rotation * targetRotOffset, Time.deltaTime * 4f);

		}
	}
}