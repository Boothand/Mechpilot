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

	public static Mech mech1, mech2;

	void Awake()
	{
		pilot = transform.root.GetComponentInChildren<Pilot>();
		weaponsOfficer = transform.root.GetComponentInChildren<WeaponsOfficer>();

		Mech[] mechs = FindObjectsOfType<Mech>();

		for (int i = 0; i < mechs.Length; ++i)
		{
			if (mechs[i].getTeam != team)
			{
				tempEnemy = mechs[i];
				break;
			}
			else
			{
				Debug.LogWarning("Two mechs exist with the same team. Change one of them.");
			}
		}

		if (team == TeamEnum.Team1)
			mech1 = this;
		else
			mech2 = this;
	}
}