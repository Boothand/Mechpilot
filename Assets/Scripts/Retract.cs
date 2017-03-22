using System.Collections;
using UnityEngine;

public class Retract : MechComponent
{
	[SerializeField] float retractDuration = 0.75f;
	public bool retracting { get; private set; }
	public delegate void NoParam();
	public event NoParam OnRetractBegin;

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

	string AnimFromStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return "Retract BL";
			case WeaponsOfficer.CombatDir.BottomRight:
				return "Retract BR";
			case WeaponsOfficer.CombatDir.Top:
				return "Retract Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Retract TL";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Retract TR";
		}

		return "Retract Top";
	}

	IEnumerator RetractRoutine()
	{
		//print("Begin retract");
		if (OnRetractBegin != null)
			OnRetractBegin();

		retracting = true;

		//IKPose targetPose = GetRetractPose(stancePicker.stance);// arms.attacker.dir);
		//IKPose targetPose2 = arms.stancePicker.GetStancePose(stancePicker.stance);

		//float duration = retractDuration / 2;

		//float retractTimer = 0f;

		//arms.StoreTargets();

		animator.CrossFade(stancePicker.AnimForStance(stancePicker.stance), stancePicker.getSwitchTime);
		//WeaponsOfficer.CombatDir stanceToUse = stancePicker.stance;

		yield return new WaitForSeconds(retractDuration);
		//while (retractTimer < duration)
		//{
		//	retractTimer += Time.deltaTime;

		//	targetPose2 = arms.stancePicker.GetStancePose(stancePicker.stance);
		//	arms.InterpolateIKPose(targetPose, retractTimer / duration);

		//	yield return null;
		//}

		//retractTimer = 0f;

		//arms.StoreTargets();
		//targetPose2 = arms.stancePicker.GetStancePose(stancePicker.stance);
		//WeaponsOfficer.CombatDir stanceToUse = stancePicker.stance;

		//while (retractTimer < duration)
		//{
		//	retractTimer += Time.deltaTime;
		//	arms.InterpolateIKPose(targetPose2, retractTimer / duration);

		//	yield return null;
		//}

		retracting = false;
		stancePicker.ForceStance(stancePicker.stance);
		arms.combatState = WeaponsOfficer.CombatState.Stance;
		//print("End retract");
		yield return null;
	}

	//public bool InRetractAnimation()
	//{
	//	AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);

	//	if (stateInfo.IsName("
	//}

	void Update()
	{
		if (!retracting && arms.combatState == WeaponsOfficer.CombatState.Retract)
		{
			StartCoroutine(RetractRoutine());
		}
	}
}