using System.Collections;
using UnityEngine;

public class Windup : MechComponent
{
	[SerializeField] float windupTime = 0.5f;
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	public Transform targetTransform { get; private set; }
	public bool windingUp { get; private set; }
	public WeaponsOfficer.CombatDir dir { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	Transform DecideWindupTransform()
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

		Transform rIK = arms.getRhandIKTarget;
		Vector3 fromPos = rIK.position;
		Quaternion fromRot = rIK.rotation;

		float timer = 0f;


		while (timer < windupTime)
		{
			timer += Time.deltaTime;

			rIK.position = Vector3.Lerp(fromPos, targetTransform.position, timer / windupTime);
			rIK.rotation = Quaternion.Lerp(fromRot, targetTransform.rotation, timer / windupTime);

			yield return new WaitForEndOfFrame();
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
			if (!windingUp && !stancePicker.changingStance)
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