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
					//Detect which 'corner' the hand is closest to.
					windupCorner = GetCornerFromHandPos();

					//Special cases.
					if (windupCorner == new Vector2(0, -1))
					{
						windupCorner = new Vector2(1, -1);
					}

					//Activate corresponding animation
					animator.SetInteger("RHand X", (int)windupCorner.x);
					animator.SetInteger("RHand Y", (int)windupCorner.y);

					animator.SetTrigger("WindUp");

					state = State.WindUp;
				}

				break;

			case State.WindUp:

				float windupDuration = 0.5f;
				windupTimer += Time.deltaTime;				

				//Wait until hand is in the pose (probably around 0.5 secs)
				if (windupTimer > windupDuration)
				{
					float inputX = input.rArmHorz;
					float inputY = input.rArmVert;

					Vector3 inputVec = new Vector3(inputX, inputY);

					//If you hold your stick close enough to a corner to warrant an attack
					if (Mathf.Abs(inputX) > 0.7f ||
						Mathf.Abs(inputY) > 0.7)
					{
						//FIXME: Circle instead of square check.
						Vector2 targetCorner = GetCorner(inputVec, 0.5f, 0.5f);
						//print("Diff: " + (targetCorner - windupCorner).magnitude.ToString("0.00") );

						//If targetcorner is not the same corner as the one you're winding up from
						if ((targetCorner - windupCorner).magnitude > 1f)
						{
							//Set state to attack.
							state = State.Attack;
							animator.SetInteger("RHand X", (int)targetCorner.x);
							animator.SetInteger("RHand Y", (int)targetCorner.y);
							animator.SetTrigger("Attack");
							windupTimer = 0f;
						}
					}
				}

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