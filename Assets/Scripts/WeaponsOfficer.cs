using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	ArmMovement armMovement;
	ArmRotation armRotation;

	protected override void OnAwake()
	{
		base.OnAwake();

		armMovement = GetComponent<ArmMovement>();
		armRotation = GetComponent<ArmRotation>();
	}
	
	void Update ()
	{
		//Move IK targets horizontally and vertically
		armMovement.RunComponent();

		//Rotate right hand
		armRotation.RunComponent();
	}
}