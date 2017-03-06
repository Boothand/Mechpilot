using System.Collections;
using UnityEngine;

public class Retract : MechComponent
{
	[SerializeField] float retractDuration = 0.75f;
	bool retracting;

	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	Transform GetRetractTransform(WeaponsOfficer.CombatDir dir)
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

	IEnumerator RetractRoutine()
	{
		Transform targetTransform = GetRetractTransform(arms.attacker.dir);

		Transform rIK = arms.getRhandIKTarget;

		float duration = retractDuration / 2;
		
		Vector3 fromPos = rIK.position;
		Quaternion fromRot = rIK.rotation;
		float retractTimer = 0f;

		while (retractTimer < duration)
		{
			retractTimer += Time.deltaTime;
			rIK.position = Vector3.Lerp(fromPos, targetTransform.position, retractTimer / duration);
			rIK.rotation = Quaternion.Lerp(fromRot, targetTransform.rotation, retractTimer / duration);

			yield return new WaitForEndOfFrame();
		}

		fromPos = rIK.position;
		fromRot = rIK.rotation;
		retractTimer = 0f;

		Transform targetTransform2 = arms.stancePicker.GetStanceTransform();

		while (retractTimer < duration)
		{
			retractTimer += Time.deltaTime;
			rIK.position = Vector3.Lerp(fromPos, targetTransform2.position, retractTimer / duration);
			rIK.rotation = Quaternion.Lerp(fromRot, targetTransform2.rotation, retractTimer / duration);

			yield return new WaitForEndOfFrame();
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