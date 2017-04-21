using System.Collections;
using UnityEngine;

public class Dodge : MechComponent
{
	public bool dodging { get; private set; }
	[SerializeField] float duration = 1f;
	enum DodgeDir { Left, Right, Back, Forward }	//Directions we can dodge.

	[SerializeField] float staminaAmount = 15f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		//Modify velocity before it is applied.
		pilot.move.ProcessVelocity += DodgeVelocityModification;
	}

	//bool SlashOnWayBack(DodgeDir dir)
	//{
	//	switch (dir)
	//	{
	//		case DodgeDir.Left:
	//			if (arms.stancePicker.stance == WeaponsOfficer.CombatDir.TopRight
	//				//|| stancePicker.stance == WeaponsOfficer.CombatDir.BottomRight
	//				)
	//			{
	//				return true;
	//			}
	//			break;

	//		case DodgeDir.Right:
	//			if (arms.stancePicker.stance == WeaponsOfficer.CombatDir.TopLeft
	//				//|| stancePicker.stance == WeaponsOfficer.CombatDir.BottomLeft
	//				)
	//			{
	//				return true;
	//			}
	//			break;

	//		case DodgeDir.Back:
	//			if (arms.stancePicker.stance == WeaponsOfficer.CombatDir.Top)
	//			{
	//				return true;
	//			}
	//			break;
	//	}

	//	return false;
	//}

	//Don't move when dodging
	void DodgeVelocityModification(ref Vector3 velocity)
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
		if (input.dodge)
		{
			if (input.moveHorz < -0.3f)
			{
				dodgeDir = DodgeDir.Left;
				animator.CrossFadeInFixedTime("Dodge Left", 0.15f);
			}
			else if (input.moveHorz > 0.3f)
			{
				dodgeDir = DodgeDir.Right;
				animator.CrossFadeInFixedTime("Dodge Right", 0.15f);
			}
			else if (input.moveVert < -0.3f)
			{
				dodgeDir = DodgeDir.Back;
				animator.CrossFadeInFixedTime("Dodge Back", 0.15f);
			}
			else if (input.moveVert > 0.3f)
			{
				dodgeDir = DodgeDir.Forward;
				//animator.CrossFadeInFixedTime("Dodge Back", 0.15f);	//No anim for it yet
			}
		}

		float dodgeTimer = 0f;

		//Wait through duration
		yield return new WaitForSeconds(dodgeTimer);

		dodgeTimer = 0f;

		//How long to hold the dodge pose

		bool releasedDodge = false;
		
		//Max time to hold pose = 0.75 seconds.
		//If you hold the stick in a direction, continue..
		while (dodgeTimer < 0.75f
			&& !releasedDodge
			&& input.dodge
			&& (Mathf.Abs(input.moveHorz) > 0.4f
			|| Mathf.Abs(input.moveHorz) > 0.4f
			|| Mathf.Abs(input.moveVert) > 0.4f)
			)
		{
			//Check if dodge direction and stick directions don't correspond,
			//if so, exit block pose.

			if (dodgeDir == DodgeDir.Right
				&& input.moveHorz < 0.4f)
			{
				releasedDodge = true;
			}
			else if (dodgeDir == DodgeDir.Left
				&& input.moveHorz > -0.4f)
			{
				releasedDodge = true;
			}
			else if (dodgeDir == DodgeDir.Back
				&& input.moveVert > -0.4f)
			{
				releasedDodge = true;
			}
			else if (dodgeDir == DodgeDir.Forward
				&& input.moveVert < 0.4f)
			{
				releasedDodge = true;
			}

			dodgeTimer += Time.deltaTime;
			yield return null;
		}

		//Finally, get back to stance animation
		animator.CrossFadeInFixedTime(arms.stancePicker.OrientationAnim(), 0.4f);

		dodging = false;
	}

	//Initiate the dodge:
	void Update()
	{
		if (!dodging
			&& input.dodge
			&& !pilot.dasher.inDash
			&& energyManager.CanSpendStamina(staminaAmount)
			//&& arms.combatState != WeaponsOfficer.CombatState.Windup
			//&& arms.combatState != WeaponsOfficer.CombatState.Attack
			)
		{
			//If you hold the stick in a direction
			if (Mathf.Abs(input.moveHorz) > 0.4f ||
				Mathf.Abs(input.moveVert) > 0.4f)
			{
				//Make sure the animator is ready or else it looks bugged.
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