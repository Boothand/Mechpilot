using System.Collections;
using UnityEngine;

public class Team
{
	public string teamName;
	public A_GlobalSettings.RoleSelection player1, player2;
	public int rewiredPlayer1, rewiredPlayer2;
}

public static class A_GlobalSettings
{
	public enum RoleSelection { Legs, Arms }
	public static Team team1, team2;


	public static void SetTeamInfo(Team newTeam1, Team newTeam2)
	{
		team1 = newTeam1;
		team2 = newTeam2;
	}
}