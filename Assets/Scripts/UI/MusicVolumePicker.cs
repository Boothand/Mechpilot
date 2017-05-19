using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumePicker : MonoBehaviour
{
	[SerializeField] AudioSource musicSource;


	public void SetVolume(Slider slider)
	{
		musicSource.volume = slider.value;
	}
}