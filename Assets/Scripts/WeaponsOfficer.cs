using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	public ArmMovement armMovement { get; private set; }
	public ArmRotation weaponControl { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		armMovement = GetComponent<ArmMovement>();
		weaponControl = GetComponent<ArmRotation>();
	}
	
	void Update ()
	{
		//Move IK targets horizontally and vertically
		armMovement.RunComponent();

		//Run attack animations, manage attacking states and gameplay
	}
}