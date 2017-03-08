using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform, bottomMidPose;
	public WeaponsOfficer.CombatDir stance { get; private set; }
	WeaponsOfficer.CombatDir prevStance;

	public IKPose targetPose { get; private set; }

	[SerializeField] float blendSpeed = 4f;
	[SerializeField] float switchTime = 0.5f;
	public bool changingStance { get; private set; }
	

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	

	public IKPose GetStancePose()
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

		arms.StoreTargets();

		if (goToMid)
		{
			while (timer < switchTimeToUse)
			{
				timer += Time.deltaTime;
				arms.InterpolateIKPose(bottomMidPose, timer / switchTimeToUse);

				yield return null;
			}

			arms.StoreTargets();
		}

		timer = 0f;

		while (timer < switchTimeToUse)
		{
			timer += Time.deltaTime;

			arms.InterpolateIKPose(targetPose, timer / switchTimeToUse);

			yield return null;
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
			targetPose = GetStancePose();

			if (prevStance != stance)
			{
				StopAllCoroutines();
				StartCoroutine(ChangeStanceRoutine(stance));
			}

			if (!changingStance)
			{
				//Transform rIK = arms.getRhandIKTarget;

				//if (Vector3.Distance(rIK.position, targetPose.rHand.position) > 0.01f)
				//{
				//	//arms.StoreTargets();
				//	//arms.InterpolateIKPose(targetPose, Time.deltaTime * blendSpeed);
				//	rIK.position = Vector3.Lerp(rIK.position, targetPose.rHand.position, Time.deltaTime * blendSpeed);
				//	rIK.rotation = Quaternion.Lerp(rIK.rotation, targetPose.rHand.rotation, Time.deltaTime * blendSpeed);
				//}
				//else
				//{
				//	//arms.StoreTargets();
				//	//arms.InterpolateIKPose(targetPose, 1f);
				//	rIK.position = targetPose.rHand.position;
				//	rIK.rotation = targetPose.rHand.rotation;
				//}
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