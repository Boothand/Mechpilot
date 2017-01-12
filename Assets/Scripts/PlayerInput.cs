using UnityEngine;
using Rewired;

public class PlayerInput : AbstractInput
{
	protected Player helmPlayer;
	protected Player armsPlayer;
	protected Player engineerPlayer;
	int helmID = 0;
	int armsID = 1;
	int engineerID = 2;

	void Awake()
	{
		helmPlayer = ReInput.players.GetPlayer(helmID);
		armsPlayer = ReInput.players.GetPlayer(armsID);
		engineerPlayer = ReInput.players.GetPlayer(engineerID);
	}
	
	void Update ()
	{
		moveHorz = helmPlayer.GetAxis("Move Horizontal");
		moveVert = helmPlayer.GetAxis("Move Vertical");
		lookHorz = helmPlayer.GetAxis("Look Horizontal");
		lookVert = helmPlayer.GetAxis("Look Vertical");

		lArmHorz = armsPlayer.GetAxis("Move Left Arm X");
		lArmVert = armsPlayer.GetAxis("Move Left Arm Y");
		rArmHorz = armsPlayer.GetAxis("Move Right Arm X");
		rArmVert = armsPlayer.GetAxis("Move Right Arm Y");

		rArmRot = armsPlayer.GetAxis("Rotate Hand");

		/*crouch = rewiredPlayer.GetButtonDown(crouchString);

		attack = rewiredPlayer.GetButtonDown(attackString);

		engineerHorz = rewiredPlayer.GetAxis(engineerHorzString);
		engineerVert = rewiredPlayer.GetAxis(engineerVertString);*/


		droneSide = armsPlayer.GetAxis("Scout Drone Horizontal");
		droneForward = armsPlayer.GetAxis("Scout Drone Vertical");
		droneDrive = armsPlayer.GetAxis("Scout Drone Drive");
		dronePowerslide = armsPlayer.GetButton("Scout Drone Powerslide");
	}
}