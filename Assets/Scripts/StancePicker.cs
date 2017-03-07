using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform, bottomMidPoseTransform;
	public WeaponsOfficer.CombatDir stance { get; private set; }
	WeaponsOfficer.CombatDir prevStance;

	public Transform targetTransform { get; private set; }

	[SerializeField] float blendSpeed = 4f;
	[SerializeField] float switchTime = 0.5f;
	public bool changingStance { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	

	public Transform GetStanceTransform()
	{
		switch (stance)
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

	public void Stop()
	{
		StopAllCoroutines();
		changingStance = false;
	}

	IEnumerator ChangeStanceRoutine(WeaponsOfficer.CombatDir newStance)
	{
		changingStance = true;
		bool goToMid = false;
		Transform rIK = arms.getRhandIKTarget;
		Vector3 fromPos = rIK.position;
		Quaternion fromRot = rIK.rotation;
		float timer = 0f;

		float switchTimeToUse = switchTime;

		if (prevStance != WeaponsOfficer.CombatDir.Top && newStance != WeaponsOfficer.CombatDir.Top)
		{
			if (prevStance == WeaponsOfficer.CombatDir.BottomLeft && newStance == WeaponsOfficer.CombatDir.BottomRight ||
				prevStance == WeaponsOfficer.CombatDir.BottomRight && newStance == WeaponsOfficer.CombatDir.BottomLeft ||
				prevStance == WeaponsOfficer.CombatDir.TopLeft && newStance == WeaponsOfficer.CombatDir.TopRight ||
				prevStance == WeaponsOfficer.CombatDir.TopRight && newStance == WeaponsOfficer.CombatDir.TopLeft ||
				prevStance == WeaponsOfficer.CombatDir.BottomLeft && newStance == WeaponsOfficer.CombatDir.TopRight ||
				prevStance == WeaponsOfficer.CombatDir.BottomRight && newStance == WeaponsOfficer.CombatDir.TopLeft ||
				prevStance == WeaponsOfficer.CombatDir.TopLeft && newStance == WeaponsOfficer.CombatDir.BottomRight ||
				prevStance == WeaponsOfficer.CombatDir.TopRight && newStance == WeaponsOfficer.CombatDir.BottomLeft)
			{
				goToMid = true;
				switchTimeToUse /= 2f;
			}
		}

		if (goToMid)
		{
			while (timer < switchTimeToUse)
			{
				timer += Time.deltaTime;

				rIK.position = Vector3.Lerp(fromPos, bottomMidPoseTransform.position, timer / switchTimeToUse);
				rIK.rotation = Quaternion.Lerp(fromRot, bottomMidPoseTransform.rotation, timer / switchTimeToUse);

				yield return new WaitForEndOfFrame();
			}
		}

		timer = 0f;
		fromPos = rIK.position;
		fromRot = rIK.rotation;

		while (timer < switchTimeToUse)
		{
			timer += Time.deltaTime;

			rIK.position = Vector3.Lerp(fromPos, targetTransform.position, timer / switchTimeToUse);
			rIK.rotation = Quaternion.Lerp(fromRot, targetTransform.rotation, timer / switchTimeToUse);

			yield return new WaitForEndOfFrame();
		}

		changingStance = false;
	}

	void Update()
	{
		if (!changingStance)
		{
			stance = arms.DecideCombatDir(stance);
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			targetTransform = GetStanceTransform();

			if (prevStance != stance)
			{
				StopAllCoroutines();
				StartCoroutine(ChangeStanceRoutine(stance));
			}

			if (!changingStance)
			{
				Transform rIK = arms.getRhandIKTarget;

				if (Vector3.Distance(rIK.position, targetTransform.position) > 0.01f)
				{
					rIK.position = Vector3.Lerp(rIK.position, targetTransform.position, Time.deltaTime * blendSpeed);
					rIK.rotation = Quaternion.Lerp(rIK.rotation, targetTransform.rotation, Time.deltaTime * blendSpeed);
				}
				else
				{
					rIK.position = targetTransform.position;
					rIK.rotation = targetTransform.rotation;
				}
			}
		}

		
		//if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		//{
		//	stance = arms.DecideCombatDir(stance);

		//	targetTransform = GetStanceTransform();

		
		//}

		prevStance = stance;
	}
}