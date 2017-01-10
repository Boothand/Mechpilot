using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	ArmMovement armMovement;

	protected override void OnAwake()
	{
		base.OnAwake();

		armMovement = GetComponent<ArmMovement>();
	}
	
	void Update ()
	{
		//Move IK targets horizontally and vertically
		armMovement.RunComponent();
	}
}