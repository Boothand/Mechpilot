using UnityEngine;

public class PlayerInput : AbstractInput
{
	[SerializeField] string moveHorzString = "Move Horizontal";
	[SerializeField] string moveVertString = "Move Vertical";

	[SerializeField] string lArmHorzString = "LeftArmHorz";
	[SerializeField] string lArmVertString = "LeftArmVert";
	[SerializeField] string rArmHorzString = "RightArmHorz";
	[SerializeField] string rArmVertString = "RightArmVert";

	[SerializeField] string lookHorzString = "Look Horizontal";
	[SerializeField] string lookVertString = "Look Vertical";

	[SerializeField] string crouchString = "Crouch";
	[SerializeField] string attackString = "Fire1";

	[SerializeField] string engineerHorzString = "EngineerHorz";
	[SerializeField] string engineerVertString = "EngineerVert";

	
	void Update ()
	{
		moveHorz = rewiredPlayer.GetAxis(moveHorzString);
		moveVert = rewiredPlayer.GetAxis(moveVertString);

		//lArmHorz = Input.GetAxis(lArmHorzString);
		//lArmVert = Input.GetAxis(lArmVertString);
		//rArmHorz = Input.GetAxis(rArmHorzString);
		//rArmVert = Input.GetAxis(rArmVertString);

		lookHorz = rewiredPlayer.GetAxis(lookHorzString);
		lookVert = rewiredPlayer.GetAxis(lookVertString);

		/*crouch = rewiredPlayer.GetButtonDown(crouchString);

		attack = rewiredPlayer.GetButtonDown(attackString);

		engineerHorz = rewiredPlayer.GetAxis(engineerHorzString);
		engineerVert = rewiredPlayer.GetAxis(engineerVertString);*/
	}
}