//using System.Collections;
using UnityEngine;

public class WeaponControl : MechComponent
{
	public enum State
	{
		Idle,
		Defend,
		WindUp,
		Attack
	}

	State state = State.Defend;

	Vector2 windupCorner;
	float attackHeight;
	float attackTimer;
	float windupTimer;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	Vector2 GetCorner(Vector3 value, float xThreshold, float yThreshold)
	{
		int x = (int)Mathf.Sign(value.x);
		int y = 0;

		//Upper or bottom corners
		if (Mathf.Abs(value.y) > yThreshold)
		{
			y = (int)Mathf.Sign(value.y);
		}

		//Top neutral
		if (y != 0 && Mathf.Abs(value.x) < xThreshold)
		{
			x = 0;
		}

		Vector2 corner = new Vector2(x, y);
		//print(corner.ToString("0"));

		return corner;
	}

	Vector2 GetCornerFromHandPos()
	{
		Vector3 center = mech.transform.InverseTransformPoint(arms.armMovement.rHandCenterPos);
		Vector3 rHandPos = mech.transform.InverseTransformPoint(arms.armMovement.rHandIK.position);

		//Difference between central hand position and current hand position
		Vector3 diff = rHandPos - center;

		float yThreshold = 0.035f / 2;
		float xThreshold = 0.035f / 2;

		return GetCorner(diff, xThreshold, yThreshold);
	}

	void Update()
	{
		switch (state)
		{
			case State.Idle:

				break;

			case State.Defend:
				if (input.attack)
				{


					state = State.WindUp;
				}

				break;

			case State.WindUp:

				

				if (!input.attack)
				{
					state = State.Defend;
					windupTimer = 0f;
				}

				break;

			case State.Attack:

				//After the attack, when animation or hand pos is in the right place, set state to WindUp
				float attackDuration = 1f;

				attackTimer += Time.deltaTime;

				if (attackTimer > attackDuration)
				{
					attackTimer = 0f;
					state = State.Defend;
				}

				if (!input.attack)
				{
					state = State.Defend;
					attackTimer = 0f;
				}

				break;
		}
	}
}