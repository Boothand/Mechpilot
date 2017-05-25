using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Base class for bars like health- and stamina bars.
public class Bar : MechComponent
{
	//The object to scale up and down
	[SerializeField] protected Transform bar;
	protected Image img;
	protected Color startColor;

	public enum Axis { X, Y }
	[SerializeField] protected Axis axis;

	public Mech.TeamEnum team;

	Coroutine blinkRoutineInstance;

	protected override void OnAwake()
	{
		if (mech == null)
		{

			Mech[] mechs = FindObjectsOfType<Mech>();

			print(mechs.Length);
			for (int i = 0; i < mechs.Length; ++i)
			{
				if (mechs[i].getTeam == team)
				{
					mech = mechs[i];
					break;
				}
			}
		}

		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		img = bar.GetComponent<Image>();

		if (img)
			startColor = img.color;
	}

	//All bars should be able to blink/flash
	protected void Blink(Vector3 location)
	{
		if (img && RoundManager.instance.roundState != RoundManager.RoundState.Ended)
		{
			if (blinkRoutineInstance != null)
				StopCoroutine(blinkRoutineInstance);

			blinkRoutineInstance = StartCoroutine(BlinkRoutine());
		}
	}

	//Tween to white and back again over a duration
	IEnumerator BlinkRoutine()
	{
		float timer = 0f;
		float duration = 0.05f;
		Color fromColor = img.color;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			img.color = Color.Lerp(fromColor, Color.white, timer / duration);
			yield return null;
		}

		duration = 0.3f;
		fromColor = img.color;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			img.color = Color.Lerp(fromColor, startColor, timer / duration);
			yield return null;
		}
	}
}