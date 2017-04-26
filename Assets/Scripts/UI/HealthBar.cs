//using System.Collections;
using UnityEngine;

//Displayed over heads and possibly somewhere else.
public class HealthBar : Bar
{

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();
		
		//Run the base blink function when the mech is hit.
		healthManager.OnGetHit += Blink;
	}

	protected override void OnUpdate()
	{
		//Scale the healthbar from 0 - 1.
		Vector3 healthScale = bar.transform.localScale;

		float targetScale = (float)healthManager.getHealth / (float)healthManager.getMaxHealth;
		switch (axis)
		{
			case Axis.X:
				healthScale.x = Mathf.Lerp(healthScale.x, targetScale, Time.deltaTime * 4f);

				break;

			case Axis.Y:
				healthScale.y = Mathf.Lerp(healthScale.y, targetScale, Time.deltaTime * 4f);
				break;
		}
		

		bar.localScale = healthScale;
	}
}