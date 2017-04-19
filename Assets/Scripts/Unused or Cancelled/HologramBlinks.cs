using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class HologramBlinks : MonoBehaviour
{
	[SerializeField] Color baseColor;
	[SerializeField] float blinkSpeed = 0.1f;
	[SerializeField] float colorVariance = 0.4f;
	RawImage img;

	void Start()
	{
		img = GetComponent<RawImage>();
		StartCoroutine(BlinkRoutine());
	}

	IEnumerator BlinkRoutine()
	{
		while (true)
		{
			Color imgColor = img.color;
			imgColor = baseColor;
			imgColor *= Random.Range(1 - colorVariance, 1 + colorVariance);
			imgColor.a = baseColor.a;

			img.color = imgColor;
			yield return new WaitForSeconds(blinkSpeed);
		}
	}
}