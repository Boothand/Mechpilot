using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Networker : NetworkManager
{
	NetworkManager m_netManager;

	void Awake()
	{
		m_netManager = GetComponent<NetworkManager>();
	}

	void Start ()
	{
		
	}

	public override void OnStartServer()
	{
		base.OnStartServer();

		//SpawnObjects();
	}

	public void SpawnObjects()
	{
		foreach (GameObject go in m_netManager.spawnPrefabs)
		{
			Network.Instantiate(go, Vector3.one * Random.Range(-2f, 2f), go.transform.rotation, 0);
		}
	}
	
	void Update ()
	{
		foreach (NetworkPlayer player in Network.connections)
		{
			Debug.Log(player.ipAddress + " is connected");
		}
	}
}