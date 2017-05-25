using System.Collections;
using UnityEngine;

public class MultimonitorPicker : MonoBehaviour
{
	
	public void SetMultiMonitorMode(UnityEngine.UI.Toggle toggle)
	{
		A_GlobalSettings.useMultiMonitor = toggle.isOn;
	}
}