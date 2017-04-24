using UnityEngine;

//This class is used to provide 'abstract' input to any class - its main advantage is that we can simulate input with AI.
//Each field corresponds to an input that can be bound to anything on the controller.
public class AbstractInput : MonoBehaviour
{
	[HideInInspector] public bool restartScene;
	//---------- PILOT ---------- \\

	//Leg movement
	[HideInInspector] public float moveHorz;
	[HideInInspector] public float moveVert;

	//Looking

	[HideInInspector] public float turnBodyHorz;
	[HideInInspector] public float turnBodyVert;

	[HideInInspector] public float crouchAxis;
	[HideInInspector] public bool dodge;
	[HideInInspector] public bool dash;
	[HideInInspector] public bool kick;
	[HideInInspector] public bool run;
	[HideInInspector] public bool jump;

	[HideInInspector] public bool giveToWeapons;
	[HideInInspector] public bool takeFromWeapons;

	[HideInInspector] public bool lockOn;

	//Camera control
	[HideInInspector] public bool camLeft, camRight, camBehind, camFP;


	//---------- WEAPONS OFFICER ---------- \\
	//Left arm movement
	[HideInInspector] public float lArmHorz;
	[HideInInspector] public float lArmVert;

	//Right arm movement
	[HideInInspector] public float rArmHorz;
	[HideInInspector] public float rArmVert;

	//Right arm rotation
	[HideInInspector] public float rArmRot;

	[HideInInspector] public bool attack;
	[HideInInspector] public bool block;

	[HideInInspector] public bool giveToPilot;
	[HideInInspector] public bool takeFromPilot;

	//---------- Drones ----------
	[HideInInspector] public float droneForward;
	[HideInInspector] public float droneSide;
	[HideInInspector] public float droneDrive;
	[HideInInspector] public bool dronePowerslide;
}