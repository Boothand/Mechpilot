using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class MPMatchMenu : Menu
{
	[SerializeField] PlayerSelectionUI team1, team2;


	public void TestRumble(int controllerIndex)
	{
		foreach (Joystick j in ReInput.players.GetPlayer(controllerIndex).controllers.Joysticks)
		{
			if (!j.supportsVibration)
				continue;

			if (j.vibrationMotorCount > 0)
				j.SetVibration(0, 1f, 1.0f); // 1 second duration
		}
	}

	public void StartMatch()
	{
		A_GlobalSettings.SetTeamInfo(team1.team, team2.team);

		SceneManager.LoadScene("DM1");
	}
}