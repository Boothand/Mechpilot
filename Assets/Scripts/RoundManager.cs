using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
	public enum RoundState
	{ Countdown, Started, Ended }
	public RoundState roundState { get; set; }

	public System.Action OnCountdownBegin;
	public System.Action OnRoundStarted;
	public System.Action OnRoundEnded;

	public static RoundManager instance;

	const int requiredWins = 3;

	[SerializeField] AudioClip three, two, one, go;

	[SerializeField] TMPro.TMP_Text countdownText;
	[SerializeField] TMPro.TMP_Text scoreCounter;

	[SerializeField] bool skipCountdown;

	//Keep them here and static, so they don't get reset when resetting the scene and its instances.
	public static int mech1RoundWins, mech2RoundWins;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	void Start()
	{
		StartCoroutine(StartRoundRoutine());

		scoreCounter.text = string.Format("{0} - {1}", mech1RoundWins, mech2RoundWins);
	}

	IEnumerator StartRoundRoutine()
	{
		yield return new WaitForSeconds(1f);

		float countdownTime = 3f;

		float timer = countdownTime;

		if (OnCountdownBegin != null)
			OnCountdownBegin();

		countdownText.gameObject.SetActive(true);

		int prevNum = 0;
		AudioClip numClip = three;

		while (timer > 0f && !skipCountdown)
		{
			int currentNum = Mathf.CeilToInt(timer);
			countdownText.text = currentNum.ToString();
			timer -= Time.deltaTime;

			//Quick way to play an audioclip for each number in the countdown
			if (currentNum != prevNum)
			{
				if (currentNum == 2)
					numClip = two;
				else if (currentNum == 1)
					numClip = one;

				AudioSource.PlayClipAtPoint(numClip, Mech.mech1.transform.position, 1f);

			}

			prevNum = currentNum;
			yield return null;
		}

		countdownText.text = "GO!";
		AudioSource.PlayClipAtPoint(go, Mech.mech1.transform.position, 1f);

		roundState = RoundState.Started;

		if (OnRoundStarted != null)
			OnRoundStarted();

		yield return new WaitForSeconds(1f);

		countdownText.gameObject.SetActive(false);

	}

	public void EndRound(Mech winner)
	{
		if (roundState != RoundState.Ended)
		{
			if (winner.getTeam == Mech.TeamEnum.Team1)
				mech1RoundWins++;
			else
				mech2RoundWins++;

			scoreCounter.text = string.Format("{0} - {1}", mech1RoundWins, mech2RoundWins);

			roundState = RoundState.Ended;

			if (mech1RoundWins < requiredWins &&
				mech2RoundWins < requiredWins)
			{
				StartCoroutine(EndRoundRoutine(winner));
			}
			else
			{
				StartCoroutine(DeclareGameWinner(winner));
			}
		}
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	IEnumerator EndRoundRoutine(Mech winner)
	{
		yield return new WaitForSeconds(2f);

		//Place the victory camera.
		VictoryCamera.instance.LookAt(winner);
		VictoryCamera.instance.gameObject.SetActive(true);

		//Hide UI overlay for mechs.
		IngameUIOverlay.instance.gameObject.SetActive(false);

		//Draw UI stuff to declare the winning team.
		VictoryUI.instance.DisplayRoundWinnerUI(winner.getTeam);

		yield return new WaitForSeconds(4f);

		//Wait a couple of seconds, start next round.
		RestartScene();
	}

	IEnumerator DeclareGameWinner(Mech winner)
	{
		yield return new WaitForSeconds(2f);

		//Place the victory camera.
		VictoryCamera.instance.LookAt2(winner);
		VictoryCamera.instance.gameObject.SetActive(true);

		//Hide UI overlay for mechs.
		IngameUIOverlay.instance.gameObject.SetActive(false);

		//Draw UI stuff to declare the winning team.
		VictoryUI.instance.DisplayMatchWinnerUI(winner.getTeam);

		yield return new WaitForSeconds(4f);

		//Wait a couple of seconds, start next round.
		GoToMainMenu();
	}

	void Update()
	{
		
	}
}