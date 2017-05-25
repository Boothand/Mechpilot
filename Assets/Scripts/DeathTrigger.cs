using System.Collections;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
	


	void Awake()
	{
		
	}

	void OnTriggerEnter(Collider col)
	{
		Mech mech = col.transform.root.GetComponentInChildren<Mech>();

		print("Found something");
		
		if (mech != null)
		{
			mech.pilot.healthManager.Die();
		}
	}

	void Update()
	{
		
	}
}