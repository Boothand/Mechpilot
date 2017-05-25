using System.Collections;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
	public static VictoryUI instance;
	[SerializeField] TMPro.TMP_Text winnerText;
	[SerializeField] Transform victoryV;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		gameObject.SetActive(false);
	}

	public void DisplayRoundWinnerUI(Mech.TeamEnum winnerTeam)
	{
		gameObject.SetActive(true);

		string teamName = "Team 1";

		if (winnerTeam == Mech.TeamEnum.Team2)
			teamName = "Team 2";

		if (A_GlobalSettings.team1 != null &&
			A_GlobalSettings.team2 != null)
		{
			teamName = A_GlobalSettings.team1.teamName;

			if (winnerTeam == Mech.TeamEnum.Team2)
				teamName = A_GlobalSettings.team2.teamName;
		}		

		winnerText.text = string.Format("{0} is victorious", teamName);
		StartCoroutine(ScaleVictoryV());
	}

	public void DisplayMatchWinnerUI(Mech.TeamEnum winnerTeam)
	{
		gameObject.SetActive(true);

		string teamName = "Team 1";

		if (winnerTeam == Mech.TeamEnum.Team2)
			teamName = "Team 2";

		if (A_GlobalSettings.team1 != null &&
			A_GlobalSettings.team2 != null)
		{
			teamName = A_GlobalSettings.team1.teamName;

			if (winnerTeam == Mech.TeamEnum.Team2)
				teamName = A_GlobalSettings.team2.teamName;
		}

		winnerText.text = string.Format("{0} wins the match", teamName);
		StartCoroutine(ScaleVictoryV());
	}

	IEnumerator ScaleVictoryV()
	{
		Vector3 startScale = victoryV.localScale;
		Vector3 endScale = startScale * 1.5f;
		CanvasGroup canvasGroup1 = victoryV.GetComponent<CanvasGroup>();
		CanvasGroup canvasGroup2 = winnerText.GetComponent<CanvasGroup>();

		float timer = 0f;
		float duration = 3f;

		while (timer < duration)
		{
			timer += Time.deltaTime;

			victoryV.localScale = Vector3.Lerp(startScale, endScale, timer / duration);

			if (canvasGroup1 != null)
				canvasGroup1.alpha = Mathf.Lerp(0f, 1f, timer / duration);

			if (canvasGroup2 != null)
				canvasGroup2.alpha = Mathf.Lerp(0f, 1f, timer / duration * 2f);
			yield return null;
		}
	}
}