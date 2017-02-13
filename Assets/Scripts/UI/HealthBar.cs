//using System.Collections;
using UnityEngine;

public class HealthBar : MechComponent
{
	[SerializeField] Transform healthBar;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		Vector3 healthScale = healthBar.transform.localScale;

		float targetScale = (float)healthManager.getHealth / (float)healthManager.getMaxHealth;
		healthScale.y = Mathf.Lerp(healthScale.y, targetScale, Time.deltaTime * 4f);

		healthBar.localScale = healthScale;
	}
}