using UnityEngine;
using Rewired;

//This class is used to provide 'abstract' input to any class - player or AI controlled.
//Each field corresponds to an input that can be bound to anything on the controller.
public class AbstractInput : MonoBehaviour
{
	protected Player rewiredPlayer;
	[SerializeField] int playerID;

	//---------- Axes ----------
	//Leg movement
	[HideInInspector] public float moveHorz;
	[HideInInspector] public float moveVert;

	//Looking
	[HideInInspector] public float lookHorz;
	[HideInInspector] public float lookVert;

	//Left arm movement
	[HideInInspector] public float lArmHorz;
	[HideInInspector] public float lArmVert;

	//Right arm movement
	[HideInInspector] public float rArmHorz;
	[HideInInspector] public float rArmVert;


	//---------- Mech Actions ----------
	[HideInInspector] public bool crouch;
	[HideInInspector] public bool attack;
	[HideInInspector] public bool dodge;
	[HideInInspector] public bool dash;


	public float engineerHorz;
	public float engineerVert;

	void Awake()
	{
		rewiredPlayer = ReInput.players.GetPlayer(playerID);
	}
}