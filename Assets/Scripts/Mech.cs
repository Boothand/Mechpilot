using UnityEngine;
//using System.Collections;

//Common hub for components, but mostly used for getting type.
public class Mech : MonoBehaviour
{
	public Pilot pilot { get; private set; }
	public WeaponsOfficer weaponsOfficer { get; private set; }
	public Mech tempEnemy;

	public enum PlayerIndex { One, Two }
	public PlayerIndex playerIndex;

	void Awake()
	{
		pilot = transform.root.GetComponentInChildren<Pilot>();
		weaponsOfficer = transform.root.GetComponentInChildren<WeaponsOfficer>();

		Mech[] mechs = FindObjectsOfType<Mech>();

		for (int i = 0; i < mechs.Length; ++i)
		{
			if (mechs[i].playerIndex != playerIndex)
			{
				tempEnemy = mechs[i];
				break;
			}
			else
			{
				Debug.LogWarning("Two mechs exist with the same playerIndex. Change one of them.");
			}
		}
	}
}