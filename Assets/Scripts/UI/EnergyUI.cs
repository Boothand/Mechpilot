//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MechComponent
{
	[SerializeField] Image pilotEnergy, weaponsEnergy;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		Vector3 pilotScale = pilotEnergy.transform.localScale;
		Vector3 weaponsScale = weaponsEnergy.transform.localScale;

		pilotScale.y = energyManager.energies[energyManager.getHelmIndex];
		weaponsScale.y = energyManager.energies[energyManager.getWeaponsIndex];

		pilotEnergy.transform.localScale = pilotScale;
		weaponsEnergy.transform.localScale = weaponsScale;
	}
}