using UnityEngine;

public class PlayerInput : AbstractInput
{
	[SerializeField] string moveHorzString = "Horizontal";
	[SerializeField] string moveVertString = "Vertical";

	[SerializeField] string lArmHorzString = "LeftArmHorz";
	[SerializeField] string lArmVertString = "LeftArmVert";
	[SerializeField] string rArmHorzString = "RightArmHorz";
	[SerializeField] string rArmVertString = "RightArmVert";

	[SerializeField] string lookHorzString = "Mouse X";
	[SerializeField] string lookVertString = "Mouse Y";

	[SerializeField] string crouchString = "Crouch";
	[SerializeField] string attackString = "Fire1";
	
	void Update ()
	{
		moveHorz = Input.GetAxisRaw(moveHorzString);
		moveVert = Input.GetAxisRaw(moveVertString);

		//lArmHorz = Input.GetAxis(lArmHorzString);
		//lArmVert = Input.GetAxis(lArmVertString);
		//rArmHorz = Input.GetAxis(rArmHorzString);
		//rArmVert = Input.GetAxis(rArmVertString);

		lookHorz = Input.GetAxis(lookHorzString);
		lookVert = Input.GetAxis(lookVertString);

		crouch = Input.GetButtonDown(crouchString);

		attack = Input.GetButtonDown(attackString);
	}
}