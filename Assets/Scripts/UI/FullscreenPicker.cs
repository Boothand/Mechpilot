using UnityEngine;
using UnityEngine.UI;

public class FullscreenPicker : MonoBehaviour
{
	[SerializeField] Toggle toggle;

	void Start()
	{
		toggle.isOn = Screen.fullScreen;
	}

	public void SetFullscreen()
	{
		Screen.fullScreen = toggle.isOn;
	}
}