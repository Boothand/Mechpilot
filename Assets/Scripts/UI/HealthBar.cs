//using System.Collections;
using UnityEngine;

public class HealthBar : Bar
{

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void Start()
	{
		base.Start();
		
		healthManager.OnGetHit += Blink;
	}

	void Update()
	{
		Vector3 healthScale = bar.transform.localScale;

		float targetScale = (float)healthManager.getHealth / (float)healthManager.getMaxHealth;
		healthScale.y = Mathf.Lerp(healthScale.y, targetScale, Time.deltaTime * 4f);

		bar.localScale = healthScale;
	}
}