using UnityEngine;
//using System.Collections;

//Common hub for components, but mostly used for getting type.
public class Mech : MonoBehaviour
{
	public Pilot pilot { get; private set; }
	public WeaponsOfficer weaponsOfficer { get; private set; }
	public Mech tempEnemy;

	void Awake()
	{
		pilot = transform.root.GetComponentInChildren<Pilot>();
		weaponsOfficer = transform.root.GetComponentInChildren<WeaponsOfficer>();
	}
}