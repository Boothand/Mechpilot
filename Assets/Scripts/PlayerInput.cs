using UnityEngine;
using Rewired;

public class PlayerInput : AbstractInput
{
	protected Player helmPlayer;
	protected Player armsPlayer;
	protected Player engineerPlayer;
	[SerializeField] int helmID = 0;
	[SerializeField] int armsID = 1;
	[SerializeField] int engineerID = 2;

	[SerializeField] string lArmHorzString = "LeftArmHorz";
	[SerializeField] string lArmVertString = "LeftArmVert";
	[SerializeField] string rArmHorzString = "RightArmHorz";
	[SerializeField] string rArmVertString = "RightArmVert";

	[SerializeField] string crouchString = "Crouch";
	[SerializeField] string attackString = "Fire1";

	[SerializeField] string engineerHorzString = "EngineerHorz";
	[SerializeField] string engineerVertString = "EngineerVert";

	void Awake()
	{
		helmPlayer = ReInput.players.GetPlayer(helmID);
		armsPlayer = ReInput.players.GetPlayer(armsID);
		engineerPlayer = ReInput.players.GetPlayer(engineerID);
		print(helmPlayer.descriptiveName);
	}
	
	void Update ()
	{
		moveHorz = helmPlayer.GetAxis(0);
		moveVert = helmPlayer.GetAxis(1);

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