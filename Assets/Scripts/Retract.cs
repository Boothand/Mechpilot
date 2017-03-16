using System.Collections;
using UnityEngine;

public class Retract : MechComponent
{
	[SerializeField] float retractDuration = 0.75f;
	public bool retracting { get; private set; }

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
		retracting = true;

		IKPose targetPose = GetRetractPose(stancePicker.stance);// arms.attacker.dir);
		IKPose targetPose2 = arms.stancePicker.GetStancePose(stancePicker.stance);

		float duration = retractDuration / 2;
		
		float retractTimer = 0f;

		arms.StoreTargets();

		while (retractTimer < duration)
		{
			retractTimer += Time.deltaTime;

			targetPose2 = arms.stancePicker.GetStancePose(stancePicker.stance);
			arms.InterpolateIKPose(targetPose, retractTimer / duration);

			yield return null;
		}
		
		retractTimer = 0f;

		arms.StoreTargets();
		targetPose2 = arms.stancePicker.GetStancePose(stancePicker.stance);
		WeaponsOfficer.CombatDir stanceToUse = stancePicker.stance;

		while (retractTimer < duration)
		{
			retractTimer += Time.deltaTime;
			arms.InterpolateIKPose(targetPose2, retractTimer / duration);

			yield return null;
		}

		retracting = false;
		stancePicker.ForceStance(stanceToUse);
		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}

	void Update()
	{
		if (!retracting && arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			StartCoroutine(RetractRoutine());
		}
	}
}