using UnityEngine;
using Rewired;
using RewiredConsts;

//Gives a value to the abstract inputs from keyboard/controllers etc.
//If this was an AI, the values would be set directly in the class doing the AI logic.
public class PlayerInput : AbstractInput
{
	//Rewired player components.
	protected Player player1;
	protected Player player2;
	[SerializeField] int pilotID = 0;
	[SerializeField] int armsID = 1;

	[SerializeField] bool noMainMenu;
	

	//Quick way to keep the mech from moving on its own..
	[SerializeField] float deadzone = 0.01f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		

		if (A_GlobalSettings.team1 != null &&
			A_GlobalSettings.team2 != null)
		{
			if (mech.getTeam == Mech.TeamEnum.Team1)
			{
				print(transform.root.name);

				player1 = ReInput.players.GetPlayer(A_GlobalSettings.team1.rewiredPlayer1);
				player2 = ReInput.players.GetPlayer(A_GlobalSettings.team1.rewiredPlayer2);

				//Debug.Log("Player1: " + A_GlobalSettings.team1.rewiredPlayer1 + ". Name: " + player1.descriptiveName);
				//Debug.Log("Player2: " + A_GlobalSettings.team1.rewiredPlayer2 + ". Name: " + player2.descriptiveName);
			}
			else if (mech.getTeam == Mech.TeamEnum.Team2)
			{
				print(transform.root.name);

				player1 = ReInput.players.GetPlayer(A_GlobalSettings.team2.rewiredPlayer1);
				player2 = ReInput.players.GetPlayer(A_GlobalSettings.team2.rewiredPlayer2);
				print(player1.descriptiveName);
				print(player2.descriptiveName);
			}
		}
		else
		{
			print("asd");
			player1 = ReInput.players.GetPlayer(pilotID);
			player2 = ReInput.players.GetPlayer(armsID);
		}
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
	protected override void OnUpdate ()
	{
		base.OnUpdate();


		restartScene = player1.GetButtonDown(Action.Restart_Scene) ||
						player2.GetButtonDown(Action.Restart_Scene);

		//Pilot actions
		moveHorz = player1.GetAxis(Action.Move_Horizontal);
		SetDeadZone(ref moveHorz, deadzone);

		moveVert = player1.GetAxis(Action.Move_Vertical);
		SetDeadZone(ref moveVert, deadzone);

		turnBodyHorz = player1.GetAxis(Action.Turn_Body_Horz);
		turnBodyVert = player1.GetAxis(Action.Turn_Body_Vert);

		crouchAxis = player1.GetAxis(Action.Crouch);
		dash = player1.GetButtonDown(Action.Dash);
		dodge = player1.GetButton(Action.Dodge);
		kick = player1.GetButtonDown(Action.Kick);
		jump = player1.GetButtonDown(Action.Jump);
		run = player1.GetButton(Action.Run);
		lockOn = player1.GetButton(Action.Lock_On);

		//Pilot camera presets
		camLeft = player1.GetButtonDown(Action.Camera_Left);
		camRight = player1.GetButtonDown(Action.Camera_Right);
		camBehind = player1.GetButtonDown(Action.Camera_Behind);
		camFP = player1.GetButtonDown(Action.Camera_Firstperson);


		//Weapons officer actions

		//Used for changing direction/stance
		lArmHorz = player2.GetAxis(Action.Move_Left_Arm_X);
		lArmVert = player2.GetAxis(Action.Move_Left_Arm_Y);
		rArmHorz = player2.GetAxis(Action.Move_Right_Arm_X);
		rArmVert = player2.GetAxis(Action.Move_Right_Arm_Y);

		rArmRot = player2.GetAxis(Action.Rotate_Hand);

		attack = player2.GetButton(Action.Wind_Up_Attack);
		block = player2.GetButton(Action.Block);


		//droneSide = armsPlayer.GetAxis("Scout Drone Horizontal");
		//droneForward = armsPlayer.GetAxis("Scout Drone Vertical");
		//droneDrive = armsPlayer.GetAxis("Scout Drone Drive");
		//dronePowerslide = armsPlayer.GetButton("Scout Drone Powerslide");
	}
}