﻿using UnityEngine;
using Rewired;

public class PlayerInput : AbstractInput
{
	protected Player helmPlayer;
	protected Player armsPlayer;
	protected Player engineerPlayer;
	int helmID = 0;
	int armsID = 2;
	int engineerID = 1;

	void Awake()
	{
		helmPlayer = ReInput.players.GetPlayer(helmID);
		armsPlayer = ReInput.players.GetPlayer(armsID);
		engineerPlayer = ReInput.players.GetPlayer(engineerID);
		print(helmPlayer.descriptiveName);
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

		

		/*crouch = rewiredPlayer.GetButtonDown(crouchString);

		attack = rewiredPlayer.GetButtonDown(attackString);

		engineerHorz = rewiredPlayer.GetAxis(engineerHorzString);
		engineerVert = rewiredPlayer.GetAxis(engineerVertString);*/
	}
}