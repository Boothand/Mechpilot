//using System.Collections;
using UnityEngine;

public class EnergyGiveTake : MechComponent
{
	[SerializeField] EnergyManager.EnergyComponents type;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		bool giveEnergyInput = input.giveToWeapons;
		bool takeEnergyInput = input.takeFromWeapons;
		EnergyManager.EnergyComponents otherType = EnergyManager.EnergyComponents.Weapons;

		if (type == EnergyManager.EnergyComponents.Weapons)
		{
			giveEnergyInput = input.giveToPilot;
			takeEnergyInput = input.takeFromPilot;
			otherType = EnergyManager.EnergyComponents.Helm;
		}

		float amount = 0.10f;

		//Pressing the give energy button
		if (giveEnergyInput)
		{
			//print(type + " giving to " + otherType);
			energyManager.AddEnergy(otherType, amount);
		}

		//Pressing the take energy button
		if (takeEnergyInput && energyManager.energies[(int)otherType] > 0.30f)
		{
			//print(type + " taking from " + otherType);

			energyManager.AddEnergy(otherType, -amount);
		}
	}
}