using System.Collections;
using UnityEngine;

public class Attacker : MechComponent
{
	public bool inAttack { get; private set; }
	public enum AttackDir { BottomLeft, Left, TopLeft, Top, TopRight, Right, BottomRight }
	[SerializeField] float attackDuration = 0.75f;
	Vector3 inputVec;
	float inputVecMagnitude;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		arms.getWeapon.OnCollision -= OnSwordCollision;
		arms.getWeapon.OnCollision += OnSwordCollision;
	}

	void OnSwordCollision(Collision col)
	{
		if (col.transform.GetComponent<Sword>())
		{
			if (inAttack)
			{
				inAttack = false;
				StopAllCoroutines();
				animator.CrossFade("Idle Block", 0.25f);
			}
		}
	}

	AttackDir GetDir()
	{
		inputVec = new Vector3(input.rArmHorz, input.rArmVert).normalized;
		inputVecMagnitude = inputVec.magnitude;

		float threshold = 0.2f;

		//Top
		if (Mathf.Abs(inputVec.x) < threshold && inputVec.y > 0f)
		{
			return AttackDir.Top;
		}

		//Right
		if (inputVec.x > 0f)
		{
			if (inputVec.y > threshold)
			{
				//Top right
				return AttackDir.TopRight;
			}

			if (inputVec.y < -threshold)
			{
				//Bottom right
				return AttackDir.BottomRight;
			}

			return AttackDir.Right;
		}

		//Left
		if (inputVec.x < 0f)
		{
			if (inputVec.y > threshold)
			{
				//Top left
				return AttackDir.TopLeft;
			}

			if (inputVec.y < -threshold)
			{
				//Bottom left
				return AttackDir.BottomLeft;
			}

			return AttackDir.Left;
		}

		return AttackDir.Top;
	}

	IEnumerator Attack(AttackDir dir)
	{
		string windupAnimToUse = "Windup Top";
		string attackAnimToUse = "Top Slash";

		switch (dir)
		{
			case AttackDir.BottomLeft:
				windupAnimToUse = "Windup Bottom Left";
				attackAnimToUse = "Bottom Left Slash";
				break;

			case AttackDir.BottomRight:
				windupAnimToUse = "Windup Bottom Right";
				attackAnimToUse = "Bottom Right Slash";
				break;

			case AttackDir.Left:
				windupAnimToUse = "Windup Left";
				attackAnimToUse = "Left Slash";
				break;

			case AttackDir.Right:
				windupAnimToUse = "Windup Right";
				attackAnimToUse = "Right Slash";
				break;

			case AttackDir.Top:
				windupAnimToUse = "Windup Top";
				attackAnimToUse = "Top Slash";
				break;

			case AttackDir.TopLeft:
				windupAnimToUse = "Windup Top Left";
				attackAnimToUse = "Top Left Slash";
				break;

			case AttackDir.TopRight:
				windupAnimToUse = "Windup Top Right";
				attackAnimToUse = "Top Right Slash";
				break;
		}

		animator.CrossFade(windupAnimToUse, 0.25f, 1);


		while (input.attack)
		{
			yield return null;
		}

		animator.CrossFade(attackAnimToUse, 0.25f, 1);


		yield return new WaitForSeconds(attackDuration);

		inAttack = false;
	}

	void Update()
	{
		if (!inAttack && input.attack)
		{
			inAttack = true;
			AttackDir dir = GetDir();

			StopAllCoroutines();
			StartCoroutine(Attack(dir));
		}
	}
}