using UnityEngine;
using Rewired;

public class PlayerInput : AbstractInput
{
	protected Player helmPlayer;
	protected Player armsPlayer;
	[SerializeField] int helmID = 0;
	[SerializeField] int armsID = 1;

	void Awake()
	{
		helmPlayer = ReInput.players.GetPlayer(helmID);
		armsPlayer = ReInput.players.GetPlayer(armsID);
	}
	
	void Update ()
	{
		moveHorz = helmPlayer.GetAxis("Move Horizontal");
		moveVert = helmPlayer.GetAxis("Move Vertical");
		lookHorz = helmPlayer.GetAxis("Look Horizontal");
		lookVert = helmPlayer.GetAxis("Look Vertical");
		crouchAxis = helmPlayer.GetAxis("Crouch");
		dash = helmPlayer.GetButtonDown("Dash");
		kick = helmPlayer.GetButtonDown("Kick");
		run = helmPlayer.GetAxis("Run");

		lArmHorz = armsPlayer.GetAxis("Move Left Arm X");
		lArmVert = armsPlayer.GetAxis("Move Left Arm Y");
		rArmHorz = armsPlayer.GetAxis("Move Right Arm X");
		rArmVert = armsPlayer.GetAxis("Move Right Arm Y");

		rArmRot = armsPlayer.GetAxis("Rotate Hand");


		attack = armsPlayer.GetButton("Wind Up Attack");

		giveToPilot = armsPlayer.GetButtonDown("Give Pilot Energy");
		takeFromPilot = armsPlayer.GetButtonDown("Take Energy From Pilot");
		giveToWeapons = helmPlayer.GetButtonDown("Give Weapons Energy");
		takeFromWeapons = helmPlayer.GetButtonDown("Take Energy From Weapons");


		droneSide = armsPlayer.GetAxis("Scout Drone Horizontal");
		droneForward = armsPlayer.GetAxis("Scout Drone Vertical");
		droneDrive = armsPlayer.GetAxis("Scout Drone Drive");
		dronePowerslide = armsPlayer.GetButton("Scout Drone Powerslide");
	}
}