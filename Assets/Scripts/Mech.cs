using UnityEngine;
//using System.Collections;

//Common hub for components, but mostly used for getting type.
public class Mech : MonoBehaviour
{
	public Pilot pilot { get; private set; }
	public WeaponsOfficer weaponsOfficer { get; private set; }
	public Mech tempEnemy;
	public enum TeamEnum { Team1, Team2 }

	[SerializeField] TeamEnum team;
	public TeamEnum getTeam { get { return team; } }

	void Awake()
	{
		pilot = transform.root.GetComponentInChildren<Pilot>();
		weaponsOfficer = transform.root.GetComponentInChildren<WeaponsOfficer>();
	}
}