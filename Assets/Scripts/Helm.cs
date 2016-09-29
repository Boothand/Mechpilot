//using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class Helm : BasePlayer
{
	private Mech mech;
	Transform objToMove;

	void Start ()
	{
		//For now:
		mech = FindObjectOfType<Mech>();
	}

	[Command]
	void CmdMoveMech(float axis)
	{
		mech.RpcWalkTest(axis);
	}

	[Command]
	void CmdMechJump(float amount)
	{
		
	}

	void MoveMech(float axis)
	{
		//Local function:
		//mech.WalkTest(axis);

		//Server function:
		CmdMoveMech(axis);
	}

	void Update ()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		MoveMech(Input.GetAxisRaw("Horizontal"));

		if (Input.GetKeyDown(KeyCode.Space))
		{

		}
	}
}