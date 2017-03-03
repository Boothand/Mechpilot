using UnityEngine;
using Rewired;

public class PlayerInput : AbstractInput
{
	protected Player pilotPlayer;
	protected Player armsPlayer;
	[SerializeField] int pilotID = 0;
	[SerializeField] int armsID = 1;

	void Awake()
	{
		pilotPlayer = ReInput.players.GetPlayer(pilotID);
		armsPlayer = ReInput.players.GetPlayer(armsID);
	}
	
	void Update ()
	{
		moveHorz = pilotPlayer.GetAxis("Move Horizontal");
		moveVert = pilotPlayer.GetAxis("Move Vertical");
		lookHorz = pilotPlayer.GetAxis("Look Horizontal");
		lookVert = pilotPlayer.GetAxis("Look Vertical");
		turnBodyHorz = pilotPlayer.GetAxis("Turn Body Horz");
		turnBodyVert = pilotPlayer.GetAxis("Turn Body Vert");
		crouchAxis = pilotPlayer.GetAxis("Crouch");
		dash = pilotPlayer.GetButtonDown("Dash");
		kick = pilotPlayer.GetButtonDown("Kick");
		run = pilotPlayer.GetAxis("Run");

		lArmHorz = armsPlayer.GetAxis("Move Left Arm X");
		lArmVert = armsPlayer.GetAxis("Move Left Arm Y");
		rArmHorz = armsPlayer.GetAxis("Move Right Arm X");
		rArmVert = armsPlayer.GetAxis("Move Right Arm Y");

		rArmRot = armsPlayer.GetAxis("Rotate Hand");


		attack = armsPlayer.GetButton("Wind Up Attack");
		block = armsPlayer.GetButton("Block");


		droneSide = armsPlayer.GetAxis("Scout Drone Horizontal");
		droneForward = armsPlayer.GetAxis("Scout Drone Vertical");
		droneDrive = armsPlayer.GetAxis("Scout Drone Drive");
		dronePowerslide = armsPlayer.GetButton("Scout Drone Powerslide");
	}
}