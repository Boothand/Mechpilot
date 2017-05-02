using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Bar
{

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();
		
		//Run the base blink function when spending stamina.
		energyManager.OnSpendStamina += Blink;
	}	

	protected override void OnUpdate()
	{
		//Scale the stamina bar from 0 - 1.
		Vector3 staminaScale = bar.transform.localScale;

		float targetScale = energyManager.stamina / energyManager.getMaxStamina;

		switch (axis)
		{
			case Axis.X:
				staminaScale.x = Mathf.Lerp(staminaScale.x, targetScale, Time.deltaTime * 4f);

				break;

			case Axis.Y:
				staminaScale.y = Mathf.Lerp(staminaScale.y, targetScale, Time.deltaTime * 4f);
				break;
		}

		bar.localScale = staminaScale;
	}
}