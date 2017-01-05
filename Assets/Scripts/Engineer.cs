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


	public float[] energies;

	protected override void OnAwake()
	{
		base.OnAwake();
		energies = new float[3];

		energies[helmIndex] = 0.75f;
		energies[weaponsIndex] = 0.125f;
		energies[engineerIndex] = 0.125f;
	}

	//0.5  +	0.02 =		0.52
	//0.5 - 	0.01 =		0.49
	//0 -		0.01 =		-0.01

	//0 -		0.02 =		-0.02
	//0.5  +	0.01 =		0.51
	//0.5 + 	0.01 =		0.51

	public void AddEnergy(EnergyComponents component, float amount)
	{
		int index = (int)component;

		for (int i = 0; i < energies.Length; i++)
		{
			if (i == index) //The component to add to
			{
				energies[i] += amount;

				if (energies[i] < 0f)
				{
					for (int j = 0; j < energies.Length; j++)
					{
						if (j == index)
							continue;

						energies[j] += energies[index] / 2;
						//energies[index] += energies[index] / 2;
					}
				}
			}
			else //All other components
			{
				energies[i] -= amount / 2;

				//If there wasn't enough energy to subtract from
				if (energies[i] < 0f)
				{
					int belowIndex = i;

					//Add the part below 0 to the remaining component, subtract the double amount from the target
					for (int j = 0; j < energies.Length; j++)
					{
						if (j == belowIndex || j == index)
							continue;

						energies[j] -= energies[belowIndex];
						energies[index] += energies[belowIndex] * 2;
					}
				}
			}

			energies[i] = Mathf.Clamp01(energies[i]);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			AddEnergy(EnergyComponents.Helm, -0.02f);
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			AddEnergy(EnergyComponents.Helm, 0.02f);
		}

		float sum = energies[0] + energies[1] + energies[2];
		//print(sum);
	}
}