//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MechComponent
{
	[SerializeField] Image staminaBar;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		if (mech && staminaBar)
		{
			Vector3 staminaScale = staminaBar.transform.localScale;

			float targetScale = energyManager.stamina / energyManager.getMaxStamina;
			staminaScale.y = Mathf.Lerp(staminaScale.y, targetScale, Time.deltaTime * 2f);

			staminaBar.transform.localScale = staminaScale;
		}
	}
}