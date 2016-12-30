using UnityEngine;
//using System.Collections;

//Base class for every component associated with a mech
public class ManagedMechBehaviour : MonoBehaviour
{
	[Header("Auto References")]
	[SerializeField] protected Mech mech;

	protected virtual void Awake()
	{
		//Auto-get references if on the same component - otherwise assign in inspector
		if (!mech && transform.root.GetComponentInChildren<Mech>())
		{
			mech = transform.root.GetComponentInChildren<Mech>();
		}

		OnAwake();
	}

	protected virtual void OnAwake() { }
}