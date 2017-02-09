//using System.Collections;
using UnityEngine;

public class EnergyManager : MechComponent
{
	public enum EnergyComponents
	{
		Pilot,
		Weapons
	}

	public float[] energies { get; private set; }
	int pilotIndex = (int)EnergyComponents.Pilot;
	int weaponsIndex = (int)EnergyComponents.Weapons;

	public int getHelmIndex { get { return pilotIndex; } }
	public int getWeaponsIndex { get { return weaponsIndex; } }

	public float energySum { get { return energies[0] + energies[1]; } }



	protected override void OnAwake()
	{
		base.OnAwake();

		energies = new float[2];

		energies[pilotIndex] = 0.5f;
		energies[weaponsIndex] = 0.5f;
	}

	public void AddEnergy(EnergyComponents component, float amount)
	{
		int targetIndex = (int)component;

		for (int i = 0; i < energies.Length; i++)
		{

			if (i == targetIndex)// && //The component to add to
								 //energies[targetIndex] + amount > 0f &&	//Don't exceed 0 - 1
								 //energies[targetIndex] + amount < 1f)
			{
				energies[targetIndex] += amount;

				for (int j = 0; j < energies.Length; j++)
				{
					//Ignore the value you are changing
					if (j == targetIndex)
						continue;

					//Subtract half the amount from the other components
					energies[j] -= amount / (energies.Length - 1);

					//The component not currently being iterated:
					int otherIndex = Mathf.Abs(targetIndex + j - energies.Length);

					//If current component is below 0
					if (energies[j] < 0f)
					{
						//Decrease the other component by the amount the current is below 0
						energies[otherIndex] += energies[j];
					}
				}
			}

			//Make sure any value never exceeds the 0-1 range
			energies[i] = Mathf.Clamp01(energies[i]);
		}
	}

	void Update()
	{
		//print("1. " + energies[0].ToString("0.000") + "\n2. " + energies[1].ToString("0.000"));
	}
}