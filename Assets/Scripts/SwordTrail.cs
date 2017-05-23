using System.Collections;
using UnityEngine;

public class SwordTrail : MechComponent
{
	[SerializeField] TrailRenderer swordTrail;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		swordTrail.widthMultiplier = 0f;

		arms.attacker.OnAttackBegin += DisplayTrail;
	}

	void DisplayTrail()
	{
		if (arms.windup.windupTimer > 1f)
		{
			StopAllCoroutines();
			StartCoroutine(DisplayTrailRoutine());
		}
	}

	IEnumerator DisplayTrailRoutine()
	{
		float fadeDuration = 0.65f;
		float timer = 0f;

		float startVisibility = arms.windup.windupTimer / 2f; //2 is max..
		startVisibility *= 0.3f;

		//Fade in quickly first so it looks smoother
		while (timer < 0.25f)
		{
			timer += Time.deltaTime;
			swordTrail.widthMultiplier = Mathf.Lerp(0f, startVisibility, timer / 0.25f);
			yield return null;
		}

		timer = 0f;

		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;

			swordTrail.widthMultiplier = Mathf.Lerp(startVisibility, 0f, timer / fadeDuration);

			yield return null;
		}
	}
}