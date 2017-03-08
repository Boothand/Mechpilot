using System.Collections;
using UnityEngine;

public class Retract : MechComponent
{
	[SerializeField] float retractDuration = 0.75f;
	bool retracting;

	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	IKPose GetRetractPose(WeaponsOfficer.CombatDir dir)
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
		retracting = false;
		StopAllCoroutines();
	}

	IEnumerator RetractRoutine()
	{
		IKPose targetPose = GetRetractPose(arms.attacker.dir);

		//Transform rIK = arms.getRhandIKTarget;

		float duration = retractDuration / 2;
		
		//Vector3 fromPos = rIK.position;
		//Quaternion fromRot = rIK.rotation;
		float retractTimer = 0f;

		arms.StoreTargets();

		while (retractTimer < duration)
		{
			retractTimer += Time.deltaTime;

			arms.InterpolateIKPose(targetPose, retractTimer / duration);

			yield return null;
		}
		
		retractTimer = 0f;

		IKPose targetPose2 = arms.stancePicker.GetStancePose();
		arms.StoreTargets();

		while (retractTimer < duration)
		{
			retractTimer += Time.deltaTime;
			arms.InterpolateIKPose(targetPose2, retractTimer / duration);

			yield return null;
		}

		retracting = false;
		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}

	void Update()
	{
		if (!retracting && arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			retracting = true;
			StartCoroutine(RetractRoutine());
		}
	}
}