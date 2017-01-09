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
		print(helmPlayer.descriptiveName);
	}
	
	void Update ()
	{
		moveHorz = helmPlayer.GetAxis("Move Horizontal");
		moveVert = helmPlayer.GetAxis("Move Vertical");

		//lArmHorz = Input.GetAxis(lArmHorzString);
		//lArmVert = Input.GetAxis(lArmVertString);
		//rArmHorz = Input.GetAxis(rArmHorzString);
		//rArmVert = Input.GetAxis(rArmVertString);

		lookHorz = helmPlayer.GetAxis("Look Horizontal");
		lookVert = helmPlayer.GetAxis("Look Vertical");

		/*crouch = rewiredPlayer.GetButtonDown(crouchString);

		attack = rewiredPlayer.GetButtonDown(attackString);

		engineerHorz = rewiredPlayer.GetAxis(engineerHorzString);
		engineerVert = rewiredPlayer.GetAxis(engineerVertString);*/
	}
}