using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MechComponent
{
	[SerializeField] Transform staminaBar;
	[SerializeField] Image img;
	Color startColor;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		if (img)
			startColor = img.color;

		energyManager.OnSpendStamina -= Blink;
		energyManager.OnSpendStamina += Blink;
	}

	void Blink()
	{
		if (img)
		{
			StopAllCoroutines();
			StartCoroutine(BlinkRoutine());
		}
	}

	IEnumerator BlinkRoutine()
	{
		float timer = 0f;
		float duration = 0.05f;
		Color fromColor = img.color;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			img.color = Color.Lerp(fromColor, Color.white, timer / duration);
			yield return null;
		}

		duration = 0.3f;
		fromColor = img.color;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			img.color = Color.Lerp(fromColor, startColor, timer / duration);
			yield return null;
		}
	}

	void Update()
	{
		Vector3 staminaScale = staminaBar.transform.localScale;

		float targetScale = energyManager.stamina / energyManager.getMaxStamina;
		staminaScale.y = Mathf.Lerp(staminaScale.y, targetScale, Time.deltaTime * 4f);

		staminaBar.localScale = staminaScale;
	}
}