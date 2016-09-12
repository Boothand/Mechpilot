using UnityEngine;

public class Mech : MonoBehaviour
{
	[SerializeField]
	private Transform m_spawnPosEngineer;
	[SerializeField]
	private Transform m_spawnPosPilot;
	[SerializeField]
	private Transform m_spawnPosArms;

	public Transform SpawnPosEngineer { get { return m_spawnPosEngineer; } }
	public Transform SpawnPosPilot { get { return m_spawnPosPilot; } }
	public Transform SpawnPosEArms { get { return m_spawnPosArms; } }

	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}
}