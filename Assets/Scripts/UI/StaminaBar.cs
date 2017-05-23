using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Bar
{
	bool energyWarning;
	[SerializeField] TMPro.TMP_Text warningText;

	Coroutine warningRoutine;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();
		
		//Run the base blink function when spending stamina.
		energyManager.OnSpendStamina += Blink;

		if (warningText != null)
			warningText.gameObject.SetActive(false);
	}

	IEnumerator DisplayBlinkingWarningRoutine()
	{
		while (true)
		{
			warningText.gameObject.SetActive(!warningText.gameObject.activeSelf);
			yield return new WaitForSeconds(0.75f);
		}
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

		//Initiating and stopping the blinking stamina warning
		if (warningText != null)
		{
			if (!energyWarning)
			{
				if (energyManager.stamina < 20f)
				{
					energyWarning = true;
					warningRoutine = StartCoroutine(DisplayBlinkingWarningRoutine());
				}
			}

			if (energyWarning)
			{
				if (energyManager.stamina > 35f)
				{
					energyWarning = false;

					if (warningRoutine != null)
						StopCoroutine(warningRoutine);

					warningText.gameObject.SetActive(false);
				}
			}
		}
	}
}