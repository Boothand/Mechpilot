using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelectionUI : MonoBehaviour
{
	public Team team = new Team();
	[SerializeField] Mech.TeamEnum teamEnum;

	[SerializeField] TextMeshProUGUI textmesh1, textmesh2;

	void Start()
	{
		RewiredPlayerForRole();
	}

	void RewiredPlayerForRole()
	{
		team.rewiredPlayer1 = 0;
		team.rewiredPlayer2 = 1;
		
		if (team.player1 == A_GlobalSettings.RoleSelection.Arms)
		{
			team.rewiredPlayer1 = 1;
			team.rewiredPlayer2 = 0;
		}

		if (teamEnum == Mech.TeamEnum.Team2)
		{
			team.rewiredPlayer1 = 2;
			team.rewiredPlayer2 = 3;

			if (team.player1 == A_GlobalSettings.RoleSelection.Arms)
			{
				team.rewiredPlayer1 = 3;
				team.rewiredPlayer2 = 2;
			}
		}
	}

	public void SetPlayerRole(int playerIndex)
	{
		if (playerIndex == 1) //Player 1
		{
			if (team.player1 == A_GlobalSettings.RoleSelection.Arms)
			{
				team.player1 = A_GlobalSettings.RoleSelection.Legs;
				team.player2 = A_GlobalSettings.RoleSelection.Arms;
			}
			else
			{
				team.player1 = A_GlobalSettings.RoleSelection.Arms;
				team.player2 = A_GlobalSettings.RoleSelection.Legs;
			}
		}

		if (playerIndex == 2) //Player 2
		{
			if (team.player2 == A_GlobalSettings.RoleSelection.Arms)
			{
				team.player2 = A_GlobalSettings.RoleSelection.Legs;
				team.player1 = A_GlobalSettings.RoleSelection.Arms;
			}
			else
			{
				team.player2 = A_GlobalSettings.RoleSelection.Arms;
				team.player1 = A_GlobalSettings.RoleSelection.Legs;
			}
		}

		textmesh1.text = "Player 1: " + team.player1.ToString();
		textmesh2.text = "Player 2: " + team.player2.ToString();

		RewiredPlayerForRole();
	}

	public void SetTeamName(TMP_InputField inputField)
	{
		team.teamName = inputField.text;
	}
}