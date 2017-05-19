using UnityEngine;

public class GraphicsPresetPicker : MonoBehaviour
{
	[SerializeField] TMPro.TMP_Dropdown dropdown;
	

	void Start()
	{
		InitiateGraphicsText();
	}

	public void InitiateGraphicsText()
	{
		int selectableSettings = 6;

		while (dropdown.options.Count < selectableSettings)
		{
			dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData());
		}

		dropdown.options[0].text = "Fastest";
		dropdown.options[1].text = "Fast";
		dropdown.options[2].text = "Simple";
		dropdown.options[3].text = "Good";
		dropdown.options[4].text = "Beautiful";
		dropdown.options[5].text = "Fantastic";

		dropdown.value = QualitySettings.GetQualityLevel();
	}

	public void SetQualityPreset()
	{
		QualitySettings.SetQualityLevel(dropdown.value);
	}
}