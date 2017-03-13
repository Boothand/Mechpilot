using System.Collections;
using UnityEngine;

public class Stagger : MechComponent
{
	[SerializeField] IKPose tlPose, trPose, blPose, brPose, topPose;
	[SerializeField] float duration = 1f;
	public bool staggering { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	IKPose GetPose(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return blPose;
			case WeaponsOfficer.CombatDir.BottomRight:
				return brPose;
			case WeaponsOfficer.CombatDir.Top:
				return topPose;
			case WeaponsOfficer.CombatDir.TopLeft:
				return tlPose;
			case WeaponsOfficer.CombatDir.TopRight:
				return trPose;
		}

		return topPose;
	}

	IEnumerator StaggerRoutine()
	{
		staggering = true;
		arms.StoreTargets();
		IKPose targetPose = GetPose(arms.attacker.dir);

		float timer = 0f;

		while (timer < duration)
		{
			timer += Time.deltaTime;

			arms.InterpolateIKPose(targetPose, timer / duration);
			yield return null;
		}
		
		arms.combatState = WeaponsOfficer.CombatState.Stance;
		staggering = false;
	}

	public void GetStaggered()
	{
		StopAllCoroutines();
		StartCoroutine(StaggerRoutine());
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stagger)
		{

		}
	}
}