//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnergySliders : MechComponent
{
	[SerializeField] Slider[] sliders = new Slider[3];
	[SerializeField] Text[] sliderValueTexts = new Text[3];
	[SerializeField] Transform energyIndicator;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void SetEnergy(int index)
	{
		EnergyManager.EnergyComponents energyIndex = (EnergyManager.EnergyComponents)index;
		float amount = sliders[index].value - energyManager.energies[index];
		energyManager.AddEnergy(energyIndex, amount);
	}

	void Update()
	{
		for (int i = 0; i < sliders.Length; i++)
		{
			sliders[i].value = energyManager.energies[i];
			sliderValueTexts[i].text = (energyManager.energies[i]).ToString("00.00%");
		}

		//Set the scale of the horizontal energy meter bar
		float lerpTime = 5f;
		Vector3 uiScale = energyIndicator.localScale;

		uiScale.x = Mathf.Lerp(uiScale.x, energyManager.energySum, Time.deltaTime * lerpTime);
		energyIndicator.localScale = uiScale;
	}
}