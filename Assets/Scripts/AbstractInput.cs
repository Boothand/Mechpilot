using UnityEngine;

//This class is used to provide 'abstract' input to any class - player or AI controlled.
//Each field corresponds to an input that can be bound to anything on the controller.
public class AbstractInput : MonoBehaviour
{
	//---------- PILOT ---------- \\

	//Leg movement
	[HideInInspector] public float moveHorz;
	[HideInInspector] public float moveVert;

	//Looking
	[HideInInspector] public float lookHorz;
	[HideInInspector] public float lookVert;

	[HideInInspector] public float turnBodyHorz;
	[HideInInspector] public float turnBodyVert;

	[HideInInspector] public float crouchAxis;
	[HideInInspector] public bool dodge;
	[HideInInspector] public bool dash;
	[HideInInspector] public bool kick;
	[HideInInspector] public float run;

	[HideInInspector] public bool giveToWeapons;
	[HideInInspector] public bool takeFromWeapons;


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
	[HideInInspector] public bool hangBlock;

	[HideInInspector] public bool giveToPilot;
	[HideInInspector] public bool takeFromPilot;

	//---------- Drones ----------
	[HideInInspector] public float droneForward;
	[HideInInspector] public float droneSide;
	[HideInInspector] public float droneDrive;
	[HideInInspector] public bool dronePowerslide;
}