using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
{
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;
	IKPose targetPose;
	Vector3 targetPosOffset;
	Quaternion targetRotOffset;
	WeaponsOfficer.CombatDir blockStance;
	WeaponsOfficer.CombatDir idealBlock;

	[SerializeField] float minBlockTime = 0.5f;

	[SerializeField] float blendSpeed = 1f;

	[SerializeField] bool autoBlock;
	public bool blocking { get; private set; }

	public Mech tempEnemy;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	IKPose GetTargetPose(WeaponsOfficer.CombatDir dir)
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

		
		if (tempEnemy.weaponsOfficer.combatState == WeaponsOfficer.CombatState.Attack &&
			blockStance == idealBlock)
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

	IEnumerator BlockRoutine()
	{
		arms.StoreTargets();

		float timer = 0f;
		float duration = 0.75f;

		while (timer < duration)
		{
			timer += Time.deltaTime;

			arms.InterpolateIKPose(targetPose, timer / duration);
			yield return null;
		}
	}

	IEnumerator BlockTimingRoutine()
	{
		yield return new WaitForSeconds(minBlockTime);

		while (input.block)
		{
			yield return null;
		}

		blocking = false;
	}

	void Update()
	{
		if (!blocking && input.block)
		{
			blocking = true;

			stancePicker.Stop();
			windup.Stop();
			attacker.Stop();
			retract.Stop();
			stagger.Stop();
			StartCoroutine(BlockTimingRoutine());
			arms.combatState = WeaponsOfficer.CombatState.Block;
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Block)
		{
			idealBlock = DecideBlockStance(tempEnemy.weaponsOfficer.stancePicker.stance);

			if (autoBlock)
			{
				blockStance = idealBlock;
			}
			else
			{
				blockStance = stancePicker.stance;
			}

			targetPose = GetTargetPose(blockStance);
			
			AdjustPosition();

			//Only for the sake of maintaining crouch height atm
			arms.StoreTargets();
			arms.InterpolateIKPose(targetPose, targetPosOffset, Time.deltaTime * blendSpeed);
		}
	}
}