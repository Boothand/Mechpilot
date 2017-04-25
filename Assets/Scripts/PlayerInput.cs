using UnityEngine;
using Rewired;
using RewiredConsts;

//Gives a value to the abstract inputs from keyboard/controllers etc.
//If this was an AI, the values would be set directly in the class doing the AI logic.
public class PlayerInput : AbstractInput
{
	//Rewired player components.
	protected Player pilotPlayer;
	protected Player armsPlayer;
	[SerializeField] int pilotID = 0;
	[SerializeField] int armsID = 1;

	//Quick way to keep the mech from moving on its own..
	[SerializeField] float deadzone = 0.01f;

	void Awake()
	{
		pilotPlayer = ReInput.players.GetPlayer(pilotID);
		armsPlayer = ReInput.players.GetPlayer(armsID);
	}

	//Takes a value, makes sure it's not within a threshold
	void SetDeadZone(ref float value, float deadzone)
	{
		if (value > 0f)
		{
			if (value < deadzone)
				value = 0f;
		}
		else if (value < 0f)
		{
			if (value > -deadzone)
				value = 0f;
		}
	}
	
	//Get input from Rewired's input manager every frame. This runs before all other script components.
	//Use exported int consts from RewiredConsts because it's faster than string comparison.
	void Update ()
	{
		restartScene = pilotPlayer.GetButtonDown(Action.Restart_Scene) ||
						armsPlayer.GetButtonDown(Action.Restart_Scene);

		//Pilot actions
		moveHorz = pilotPlayer.GetAxis(Action.Move_Horizontal);
		SetDeadZone(ref moveHorz, deadzone);

		moveVert = pilotPlayer.GetAxis(Action.Move_Vertical);
		SetDeadZone(ref moveVert, deadzone);

		turnBodyHorz = pilotPlayer.GetAxis(Action.Turn_Body_Horz);
		turnBodyVert = pilotPlayer.GetAxis(Action.Turn_Body_Vert);

		crouchAxis = pilotPlayer.GetAxis(Action.Crouch);
		dash = pilotPlayer.GetButtonDown(Action.Dash);
		dodge = pilotPlayer.GetButton(Action.Dodge);
		kick = pilotPlayer.GetButtonDown(Action.Kick);
		jump = pilotPlayer.GetButtonDown(Action.Jump);
		run = pilotPlayer.GetButton(Action.Run);
		lockOn = pilotPlayer.GetButton(Action.Lock_On);

		//Pilot camera presets
		camLeft = pilotPlayer.GetButtonDown(Action.Camera_Left);
		camRight = pilotPlayer.GetButtonDown(Action.Camera_Right);
		camBehind = pilotPlayer.GetButtonDown(Action.Camera_Behind);
		camFP = pilotPlayer.GetButtonDown(Action.Camera_Firstperson);


		//Weapons officer actions

		//Used for changing direction/stance
		lArmHorz = armsPlayer.GetAxis(Action.Move_Left_Arm_X);
		lArmVert = armsPlayer.GetAxis(Action.Move_Left_Arm_Y);
		rArmHorz = armsPlayer.GetAxis(Action.Move_Right_Arm_X);
		rArmVert = armsPlayer.GetAxis(Action.Move_Right_Arm_Y);

		rArmRot = armsPlayer.GetAxis(Action.Rotate_Hand);

		attack = armsPlayer.GetButton(Action.Wind_Up_Attack);
		block = armsPlayer.GetButton(Action.Block);


		//droneSide = armsPlayer.GetAxis("Scout Drone Horizontal");
		//droneForward = armsPlayer.GetAxis("Scout Drone Vertical");
		//droneDrive = armsPlayer.GetAxis("Scout Drone Drive");
		//dronePowerslide = armsPlayer.GetButton("Scout Drone Powerslide");
	}
}