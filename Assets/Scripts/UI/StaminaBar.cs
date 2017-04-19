using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Bar
{

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void Start()
	{
		base.Start();
		
		energyManager.OnSpendStamina += Blink;
	}	

	void Update()
	{
		Vector3 staminaScale = bar.transform.localScale;

		float targetScale = energyManager.stamina / energyManager.getMaxStamina;
		staminaScale.y = Mathf.Lerp(staminaScale.y, targetScale, Time.deltaTime * 4f);

		bar.localScale = staminaScale;
	}
}