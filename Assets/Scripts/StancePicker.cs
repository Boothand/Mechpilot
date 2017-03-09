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

	void Start()
	{
		targetPose = GetStancePose();
		StartCoroutine(ChangeStanceRoutine(stance));
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

	public void ForceStance()
	{
		//prevStance = stance;
		if (prevStance != stance)
		{
			StopAllCoroutines();
			StartCoroutine(ChangeStanceRoutine(stance));
		}
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
			targetPose = GetStancePose();

			if (!changingStance && prevStance != stance)
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
	}
}