using UnityEngine;
//using System.Collections;

//Base class for every component associated with a mech.
//This class was intended for classes that needed to know about the mech, but was not a part of it,
//such as remote controlled drones.
public class ManagedMechBehaviour : MonoBehaviour
{
	[Header("Auto References")]
	[SerializeField] protected Mech mech;

	protected virtual void Awake()
	{
		//Auto-get references if on the same component - otherwise assign in inspector
		if (!mech)
		{
			mech = transform.root.GetComponentInChildren<Mech>();
		}

		OnAwake();
	}

	protected virtual void OnAwake() { }
}