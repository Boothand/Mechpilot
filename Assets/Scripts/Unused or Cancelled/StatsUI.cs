//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MechComponent
{
	[SerializeField] Transform staminaBar;
	[SerializeField] Transform healthBar;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void UpdateStaminaBar()
	{
		Vector3 staminaScale = staminaBar.transform.localScale;

		float targetScale = energyManager.stamina / energyManager.getMaxStamina;
		staminaScale.y = Mathf.Lerp(staminaScale.y, targetScale, Time.deltaTime * 4f);

		staminaBar.localScale = staminaScale;
	}

	void UpdateHealthBar()
	{
		Vector3 healthScale = healthBar.transform.localScale;

		float targetScale = (float)healthManager.getHealth / (float)healthManager.getMaxHealth;
		healthScale.y = Mathf.Lerp(healthScale.y, targetScale, Time.deltaTime * 4f);

		healthBar.localScale = healthScale;
	}

	void Update()
	{
		if (mech)
		{
			if (staminaBar)
			{
				UpdateStaminaBar();
			}

			if (healthBar)
			{
				UpdateHealthBar();
			}
		}
	}
}