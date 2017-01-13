using UnityEngine;
using System.Collections;

public class Engineer : MechComponent
{
	public enum EnergyComponents
	{
		Helm,
		Weapons,
		Engineer
	}

	public int helmIndex = (int)EnergyComponents.Helm;
	public int weaponsIndex = (int)EnergyComponents.Weapons;
	public int engineerIndex = (int)EnergyComponents.Engineer;

	public float energySum;

	public float[] energies;

	protected override void OnAwake()
	{
		base.OnAwake();
		energies = new float[3];

		energies[helmIndex] = 1f;
		energies[weaponsIndex] = 1f;
		energies[engineerIndex] = 0f;
	}

	public void AddEnergy(EnergyComponents component, float amount)
	{
		int targetIndex = (int)component;

		energies[targetIndex] += amount;
	}

	//Old function:
	/*
	public void AddEnergy(EnergyComponents component, float amount)
	{
		int targetIndex = (int)component;

		for (int i = 0; i < energies.Length; i++)
		{
			float sum = energies[helmIndex] + energies[weaponsIndex] + energies[engineerIndex];

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
					energies[j] -= amount / 2;

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
	*/

	void Update()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			AddEnergy(EnergyComponents.Weapons, -0.02f);
		}
		if (Input.GetKey(KeyCode.R))
		{
			AddEnergy(EnergyComponents.Weapons, 0.02f);
		}
		
		energySum = energies[helmIndex] + energies[weaponsIndex] + energies[engineerIndex];
	}
}