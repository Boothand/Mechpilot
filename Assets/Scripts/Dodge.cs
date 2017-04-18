using System.Collections;
using UnityEngine;

public class Dodge : MechComponent
{
	public bool dodging { get; private set; }
	[SerializeField] float duration = 1f;
	enum DodgeDir { Left, Right, Back, Forward }

	[SerializeField] float staminaAmount = 15f;

	//public float dodgeSlashWindupTimer { get; private set; }
	//public bool dodgeSlash { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	bool SlashOnWayBack(DodgeDir dir)
	{
		switch (dir)
		{
			case DodgeDir.Left:
				if (stancePicker.stance == WeaponsOfficer.CombatDir.TopRight
					//|| stancePicker.stance == WeaponsOfficer.CombatDir.BottomRight
					)
				{
					return true;
				}
				break;

			case DodgeDir.Right:
				if (stancePicker.stance == WeaponsOfficer.CombatDir.TopLeft
					//|| stancePicker.stance == WeaponsOfficer.CombatDir.BottomLeft
					)
				{
					return true;
				}
				break;

			case DodgeDir.Back:
				if (stancePicker.stance == WeaponsOfficer.CombatDir.Top)
				{
					return true;
				}
				break;
		}

		return false;
	}

	public void DodgeVelocityModification(ref Vector3 velocity)
	{
		if (dodging)
		{
			velocity = Vector3.zero;
		}
	}

	IEnumerator DodgeRoutine()
	{
		energyManager.SpendStamina(staminaAmount);

		dodging = true;
		DodgeDir dodgeDir = DodgeDir.Right;

		//Which way should we dodge? Play animation
		if (input.dodgeHorz < -0.3f)
		{
			dodgeDir = DodgeDir.Left;
			animator.CrossFadeInFixedTime("Dodge Left", 0.15f);
		}
		else if (input.dodgeHorz > 0.3f)
		{
			dodgeDir = DodgeDir.Right;
			animator.CrossFadeInFixedTime("Dodge Right", 0.15f);
		}
		else
		{
			dodgeDir = DodgeDir.Back;
			animator.CrossFadeInFixedTime("Dodge Back", 0.15f);
		}

		//Gradually turn off IK targets to let animation play out - ////OLD
		//arms.TweenIKWeight(0f, 1f);

		float dodgeTimer = 0f;

		//Wait through duration
		while (dodgeTimer < duration)
		{
			dodgeTimer += Time.deltaTime;
			//dodgeSlashWindupTimer = dodgeTimer / duration;	//Read by other classes
			//If you press attack with the correct stance, set flag
			//if (!dodgeSlash && input.attack)
			//{
			//	dodgeSlash = SlashOnWayBack(dodgeDir);
			//}

			yield return null;
		}

		dodgeTimer = 0f;

		//How long to hold the dodge pose

		bool releasedDodge = false;
		
		while (dodgeTimer < 0.75f
			&& !releasedDodge
			&& (Mathf.Abs(input.dodgeHorz) > 0.4f
			|| Mathf.Abs(input.dodgeVert) > 0.4f)
			)
		{
			if (dodgeDir == DodgeDir.Right
				&& input.dodgeHorz < 0.4f)
			{
				releasedDodge = true;
			}

			if (dodgeDir == DodgeDir.Left
				&& input.dodgeHorz > -0.4f)
			{
				releasedDodge = true;
			}

			if (dodgeDir == DodgeDir.Back
				&& input.dodgeVert > -0.4f)
			{
				releasedDodge = true;
			}

			dodgeTimer += Time.deltaTime;
			yield return null;
		}

		//If you should do a slash after the dodge
		//if (dodgeSlash)
		//{
		//	arms.combatState = WeaponsOfficer.CombatState.Attack;

		//	//Play correct animation
		//	switch (dodgeDir)
		//	{
		//		case DodgeDir.Left:
		//			animator.CrossFade("Dodge Left Slash", 0.35f);
		//			break;
		//		case DodgeDir.Right:
		//			animator.CrossFade("Dodge Right Slash", 0.35f);
		//			break;
		//		case DodgeDir.Back:
		//			animator.CrossFade("Dodge Back Slash", 0.35f);
		//			break;
		//		case DodgeDir.Forward:
		//			break;
		//	}

		//	float animDuration = 1f;
		//	float animDurationTimer = 0f;

		//	//Tune down layer 1's weight
		//	while (animDurationTimer < 0.3f)
		//	{
		//		animDurationTimer += Time.deltaTime;
		//		float value = Mathf.Lerp(1f, 0f, animDurationTimer / 0.3f);
		//		print(value);
		//		animator.SetLayerWeight(1, value);
		//		yield return null;
		//	}
		//	yield return new WaitForSeconds(animDuration - 0.6f);

		//	animDurationTimer = 0f;
		//	//Tune up layer 1's weight
		//	while (animDurationTimer < 0.3f)
		//	{
		//		animDurationTimer += Time.deltaTime;
		//		animator.SetLayerWeight(1, Mathf.Lerp(0f, 1f, animDurationTimer / 0.3f));
		//		yield return null;
		//	}

		//	arms.combatState = WeaponsOfficer.CombatState.Stance;
		//}

		//Transition back to idle if no slash
		//if (!dodgeSlash)
		//{
			animator.CrossFadeInFixedTime(stancePicker.OrientationAnim(), 0.4f);
		//}

		//Turn IK weights back up
		//arms.TweenIKWeight(1f, 0.5f);

		dodging = false;
		//dodgeSlash = false;
	}

	void Update()
	{
		if (!dodging
			//&& input.dodge
			&& !dasher.inDash
			&& energyManager.CanSpendStamina(staminaAmount)
			//&& arms.combatState != WeaponsOfficer.CombatState.Windup
			//&& arms.combatState != WeaponsOfficer.CombatState.Attack
			)
		{
			if (Mathf.Abs(input.dodgeHorz) > 0.4f ||
				Mathf.Abs(input.dodgeVert) > 0.4f)
			{
				if (!animator.IsInTransition(0))
				{
					//arms.stancePicker.Stop();
					//arms.windup.Stop();
					//arms.attacker.Stop();
					//arms.retract.Stop();
					//arms.stagger.Stop();
					//arms.combatState = WeaponsOfficer.CombatState.Stance;
					StartCoroutine(DodgeRoutine());
				}
			}
		}
	}
}