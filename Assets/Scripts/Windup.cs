using System.Collections;
using UnityEngine;

public class Windup : MechComponent
{
	[SerializeField] float windupTime = 0.5f;
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;
	public IKPose targetTransform { get; private set; }
	public bool windingUp { get; private set; }
	public WeaponsOfficer.CombatDir dir { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	IKPose DecideWindupTransform()
	{
		switch (stancePicker.stance)
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
		StopAllCoroutines();
		windingUp = false;
	}

	IEnumerator WindupRoutine(WeaponsOfficer.CombatDir dir)
	{
		windingUp = true;
		arms.combatState = WeaponsOfficer.CombatState.Windup;

		targetTransform = DecideWindupTransform();

		float timer = 0f;

		arms.StoreTargets();

		while (timer < windupTime)
		{
			timer += Time.deltaTime;

			arms.InterpolateIKPose(targetTransform, timer / windupTime);

			yield return null;
		}

		while (input.attack)
		{
			yield return null;
		}

		windingUp = false;
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			if (!windingUp
				&& !stancePicker.changingStance
				&& !dodger.dodging)
			{
				if (input.attack)
				{
					dir = stancePicker.stance;

					StopAllCoroutines();
					StartCoroutine(WindupRoutine(dir));
				}
			}
		}
	}
}