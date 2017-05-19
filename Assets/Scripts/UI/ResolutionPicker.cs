using System.Collections.Generic;
using UnityEngine;

public class ResolutionPicker : MonoBehaviour
{
	[SerializeField] TMPro.TMP_Dropdown dropdown;
	List<Resolution> resolutionList = new List<Resolution>();



	void Start()
	{
		InitiateResolutionOptions();
	}

	void InitiateResolutionOptions()
	{
		int screenResolutions = Screen.resolutions.Length;

		resolutionList.Clear();
		resolutionList.AddRange(Screen.resolutions);

		for (int i = 0; i < screenResolutions; i++)
		{
			if (dropdown.options.Count < screenResolutions)
			{
				dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData());
			}

			string resString = string.Format("{0} x {1}",
				Screen.resolutions[i].width, Screen.resolutions[i].height);

			//resolutionList.Add(Screen.resolutions[i]);

			dropdown.options[i].text = resString;
		}

		dropdown.captionText.text = string.Format("{0} x {1}",
				Screen.currentResolution.width, Screen.currentResolution.height);
	}

	public static void InitiateResolution()
	{
		Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
	}

	public void SetResolution()
	{
		int index = dropdown.value;
		Screen.SetResolution(resolutionList[index].width, resolutionList[index].height, Screen.fullScreen);
	}
}