//using System.Collections;
using UnityEngine;

public class Windup : MechComponent
{
	bool windingUp;
	bool attacking;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		if (!windingUp && input.attack)
		{
			windingUp = true;

			animator.CrossFade("Windup Left", 0.25f, 1);
		}

		if (windingUp && !input.attack)
		{
			windingUp = false;
			attacking = true;

			animator.CrossFade("Left Slash", 0.25f, 1);
		}
	}
}