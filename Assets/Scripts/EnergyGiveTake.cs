////using System.Collections;
//using UnityEngine;

//public class EnergyGiveTake : MechComponent
//{
//	[SerializeField] EnergyManager_Old.EnergyComponents type;

//	protected override void OnAwake()
//	{
//		base.OnAwake();
//	}

//	void Update()
//	{
//		bool giveEnergyInput = input.giveToWeapons;
//		bool takeEnergyInput = input.takeFromWeapons;
//		EnergyManager_Old.EnergyComponents otherType = EnergyManager_Old.EnergyComponents.Weapons;

//		if (type == EnergyManager_Old.EnergyComponents.Weapons)
//		{
//			giveEnergyInput = input.giveToPilot;
//			takeEnergyInput = input.takeFromPilot;
//			otherType = EnergyManager_Old.EnergyComponents.Pilot;
//		}

//		float amount = 0.10f;

//		//Pressing the give energy button
//		if (giveEnergyInput)
//		{
//			//print(type + " giving to " + otherType);
//			energyManager.AddEnergy(otherType, amount);
//		}

//		//Pressing the take energy button
//		if (takeEnergyInput && energyManager.energies[(int)otherType] > 0.30f)
//		{
//			//print(type + " taking from " + otherType);

//			energyManager.AddEnergy(otherType, -amount);
//		}
//	}
//}