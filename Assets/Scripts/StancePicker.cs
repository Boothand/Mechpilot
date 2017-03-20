using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	[SerializeField] public IKPose trTransform, tlTransform, brTransform, blTransform, topTransform, bottomMidPose;
	public WeaponsOfficer.CombatDir stance { get; private set; }
	public WeaponsOfficer.CombatDir prevStance { get; private set; }
	WeaponsOfficer.CombatState prevState;

	public IKPose targetPose { get; private set; }

	[SerializeField] float blendSpeed = 4f;
	[SerializeField] float switchTime = 0.5f;
	public bool changingStance { get; private set; }
	public WeaponsOfficer.CombatDir startStance;

	public delegate void NoParam();
	public event NoParam OnStanceBegin;
	

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		stance = startStance;
		targetPose = GetStancePose(stance);
		StartCoroutine(ChangeStanceRoutine(stance));
		animator.CrossFade(AnimForStance(stance), 0.25f);
	}

	string AnimForStance(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return "Stance_BL";
			case WeaponsOfficer.CombatDir.BottomRight:
				return "Stance_BR";
			case WeaponsOfficer.CombatDir.Top:
				return "Stance_Top";
			case WeaponsOfficer.CombatDir.TopLeft:
				return "Stance_TL";
			case WeaponsOfficer.CombatDir.TopRight:
				return "Stance_TR";
		}

		return "Stance_TR";
	}

	public IKPose GetStancePose(WeaponsOfficer.CombatDir dir)
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

	public void Stop()
	{
		StopAllCoroutines();
		prevStance = stance;
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
				//goToMid = true;
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
		animator.CrossFade(AnimForStance(stance), switchTimeToUse);

		while (timer < switchTimeToUse)
		{
			timer += Time.deltaTime;
			arms.InterpolateIKPose(targetPose, timer / switchTimeToUse);

			yield return null;
		}

		prevStance = stance;
		changingStance = false;
	}

	public IEnumerator ForceStanceRoutine()
	{
		arms.StoreTargets();

		float timer = 0f;
		float dur = 0.5f;

		while (timer < dur)
		{
			timer += Time.deltaTime;
			arms.InterpolateIKPose(targetPose, timer / dur);

			yield return null;
		}
	}

	public void ForceStance(WeaponsOfficer.CombatDir newStance)
	{
		stance = newStance;
		prevStance = newStance;
		//if (prevStance != stance)
		//{
		//	StopAllCoroutines();
		//	StartCoroutine(ChangeStanceRoutine(stance));
		//}
		//StopAllCoroutines();
		//StartCoroutine(ForceStanceRoutine());
	}

	void Update()
	{
		if (!changingStance)
		{
			stance = arms.DecideCombatDir(stance);
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			//Run event for others
			if (prevState != WeaponsOfficer.CombatState.Stance)
			{
				if (OnStanceBegin != null)
					OnStanceBegin();
			}

			targetPose = GetStancePose(stance);

			if (!changingStance && prevStance != stance
				&& !blocker.blocking
				)
			{
				StopAllCoroutines();
				StartCoroutine(ChangeStanceRoutine(stance));
			}

			if (!changingStance)
			{
				arms.StoreTargets();
				arms.InterpolateIKPose(targetPose, Time.deltaTime * 5f);
			}
		}

		prevState = arms.combatState;
	}
}