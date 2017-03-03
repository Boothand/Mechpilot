using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
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

	IEnumerator ChangeStanceRoutine(WeaponsOfficer.CombatDir newStance)
	{
		changingStance = true;
		Transform rIK = arms.getRhandIKTarget;
		Vector3 fromPos = rIK.position;
		Quaternion fromRot = rIK.rotation;
		float timer = 0f;

		while (timer < switchTime)
		{
			timer += Time.deltaTime;

			rIK.position = Vector3.Lerp(fromPos, targetTransform.position, timer / switchTime);
			rIK.rotation = Quaternion.Lerp(fromRot, targetTransform.rotation, timer / switchTime);

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