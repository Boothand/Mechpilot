using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MechComponent
{
	[SerializeField] protected Transform bar;
	protected Image img;
	protected Color startColor;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected virtual void Start()
	{
		img = bar.GetComponent<Image>();

		if (img)
			startColor = img.color;
	}

	protected void Blink()
	{
		if (img)
		{
			StopAllCoroutines();
			StartCoroutine(BlinkRoutine());
		}
	}

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

	void Update()
	{
		
	}
}