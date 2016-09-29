//using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BasePlayer : NetworkBehaviour
{
	protected NetworkIdentity netIdentity;

	void Start()
	{
		netIdentity = GetComponent<NetworkIdentity>();
	}
	
	void Update()
	{

	}
}