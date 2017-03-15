using System.Collections;
using UnityEngine;

public class Dodge : MechComponent
{
	public bool dodging { get; private set; }
	[SerializeField] float duration = 1f;
	enum DodgeDir { Left, Right, Back, Forward }


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	bool SlashOnWayBack(DodgeDir dir)
	{
		switch (dir)
		{
			case DodgeDir.Left:
				if (stancePicker.stance == WeaponsOfficer.CombatDir.TopRight ||
					stancePicker.stance == WeaponsOfficer.CombatDir.BottomRight)
				{
					return true;
				}
				break;

			case DodgeDir.Right:
				if (stancePicker.stance == WeaponsOfficer.CombatDir.TopLeft ||
					stancePicker.stance == WeaponsOfficer.CombatDir.BottomLeft)
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
		dodging = true;
		DodgeDir dodgeDir = DodgeDir.Right;
		bool slashOnWayBack = false;

		//Which way should we dodge? Play animation
		if (input.turnBodyHorz < -0.3f)
		{
			dodgeDir = DodgeDir.Left;
			animator.CrossFade("Dodge Left", 0.25f);
		}
		else if (input.turnBodyHorz > 0.3f)
		{
			dodgeDir = DodgeDir.Right;
			animator.CrossFade("Dodge Right", 0.25f);
		}
		else
		{
			dodgeDir = DodgeDir.Back;
			animator.CrossFade("Dodge Back", 0.25f);
		}

		//Gradually turn off IK targets to let animation play out
		arms.TweenIKWeight(0f, 1f);

		float dodgeTimer = 0f;

		//Wait through duration
		while (dodgeTimer < duration)
		{
			dodgeTimer += Time.deltaTime;

			//If you press attack with the correct stance, set flag
			if (!slashOnWayBack && input.attack)
			{
				slashOnWayBack = SlashOnWayBack(dodgeDir);
			}

			yield return null;
		}

		dodgeTimer = 0f;

		//How long to hold the dodge pose
		
		while (dodgeTimer < 0.75f && input.dodge)
		{
			dodgeTimer += Time.deltaTime;
			yield return null;
		}

		//If you should do a slash after the dodge
		if (slashOnWayBack)
		{

			arms.combatState = WeaponsOfficer.CombatState.Attack;

			//Play correct animation
			switch (dodgeDir)
			{
				case DodgeDir.Left:
					animator.CrossFade("Dodge Left Slash", 0.25f);
					break;
				case DodgeDir.Right:
					animator.CrossFade("Dodge Right Slash", 0.25f);
					break;
				case DodgeDir.Back:
					animator.CrossFade("Dodge Back Slash", 0.25f);
					break;
				case DodgeDir.Forward:
					break;
			}

			float animDuration = 1f;
			yield return new WaitForSeconds(animDuration);

			arms.combatState = WeaponsOfficer.CombatState.Stance;
		}

		//Transition back to idle if no slash
		if (!slashOnWayBack)
		{
			animator.CrossFade("Walk/Crouch", 1f);
		}

		//Turn IK weights back up
		arms.TweenIKWeight(1f, 0.5f);

		dodging = false;
	}

	void Update()
	{
		if (!dodging && input.dodge
			//&& arms.combatState != WeaponsOfficer.CombatState.Windup
			//&& arms.combatState != WeaponsOfficer.CombatState.Attack
			)
		{
			if (Mathf.Abs(input.turnBodyHorz) > 0.4f ||
				Mathf.Abs(input.turnBodyVert) > 0.4f)
			{
				if (!animator.IsInTransition(0))
				{
					arms.stancePicker.Stop();
					arms.windup.Stop();
					arms.attacker.Stop();
					//arms.retract.Stop();
					//arms.stagger.Stop();
					arms.combatState = WeaponsOfficer.CombatState.Stance;
					StartCoroutine(DodgeRoutine());
				}
			}
		}
	}
}