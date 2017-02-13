//using System.Collections;
using UnityEngine;

public class StaminaBar : MechComponent
{
	[SerializeField] Transform staminaBar;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		Vector3 staminaScale = staminaBar.transform.localScale;

		float targetScale = energyManager.stamina / energyManager.getMaxStamina;
		staminaScale.y = Mathf.Lerp(staminaScale.y, targetScale, Time.deltaTime * 4f);

		staminaBar.localScale = staminaScale;
	}
}