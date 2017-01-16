using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	public ArmMovement armMovement { get; private set; }
	public ArmRotation armRotation { get; private set; }
	public WeaponControl weaponControl { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		armMovement = GetComponent<ArmMovement>();
		armRotation = GetComponent<ArmRotation>();
		weaponControl = GetComponent<WeaponControl>();
	}
	
	void Update ()
	{
		//Move IK targets horizontally and vertically
		armMovement.RunComponent();

		//Rotate right hand
		armRotation.RunComponent();

		//Run attack animations, manage attacking states and gameplay
	}
}