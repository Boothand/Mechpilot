using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField] Camera splitscreenCamera, multimonitorCamera;
	Camera cameraToUse;

	void Awake()
	{
		
	}

	void Start()
	{
		splitscreenCamera.gameObject.SetActive(false);
		multimonitorCamera.gameObject.SetActive(false);

		cameraToUse = splitscreenCamera;

		if (A_GlobalSettings.useMultiMonitor)
		{
			cameraToUse = multimonitorCamera;
		}

		cameraToUse.gameObject.SetActive(true);
	}

	void Update()
	{
		
	}
}