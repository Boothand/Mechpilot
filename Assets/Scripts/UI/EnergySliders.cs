//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnergySliders : MechComponent
{
	[SerializeField]
	Slider helmSlider, armsSlider, engineerSlider;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void SetEnergy(int index)
	{
		Slider sliderToUse = helmSlider;

		if (index == 1)
			sliderToUse = armsSlider;
		if (index == 2)
			sliderToUse = engineerSlider;

		Engineer.EnergyComponents energyIndex = (Engineer.EnergyComponents)index;
		float amount = sliderToUse.value - engineer.energies[index];

		engineer.AddEnergy(energyIndex, amount);
	}

	void Update()
	{
		helmSlider.value = engineer.energies[0];
		armsSlider.value = engineer.energies[1];
		engineerSlider.value = engineer.energies[2];
	}
}