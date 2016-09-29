using UnityEngine;
using UnityEngine.Networking;

public class Mech : NetworkBehaviour
{
	[SerializeField]
	private Transform m_spawnPosEngineer;
	[SerializeField]
	private Transform m_spawnPosPilot;
	[SerializeField]
	private Transform m_spawnPosArms;

	Animator anim;
	float walkMovement;

	public Transform SpawnPosEngineer { get { return m_spawnPosEngineer; } }
	public Transform SpawnPosPilot { get { return m_spawnPosPilot; } }
	public Transform SpawnPosEArms { get { return m_spawnPosArms; } }

	void Start ()
	{
		anim = GetComponent<Animator>();
	}

	[ClientRpc]
	public void RpcWalkTest(float axis)
	{
		WalkTest(axis);
	}

	public void WalkTest(float axis)
	{
		walkMovement = Mathf.MoveTowards(walkMovement, axis, Time.deltaTime * 4f);
		anim.SetFloat("WalkSpeed", walkMovement);
		transform.Translate(Vector3.forward * axis * Time.deltaTime);
	}

	public void ChangeColorTest(Color newColor)
	{
		transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = newColor;
	}
	
	void Update ()
	{
		
	}
}