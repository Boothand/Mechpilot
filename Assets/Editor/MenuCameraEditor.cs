using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuCameraRail))]
public class MenuCameraEditor : Editor
{



	void OnSceneGUI()
	{
		MenuCameraRail menuCam = target as MenuCameraRail;

		for (int i = 0; i < menuCam.waypoints.Length; i++)
		{
			int index = i;
			int nextIndex = i + 1;

			if (nextIndex == menuCam.waypoints.Length)
				nextIndex = 0;

			Handles.DrawBezier(menuCam.waypoints[index].waypoint.position, menuCam.waypoints[nextIndex].waypoint.position,
							menuCam.waypoints[index].handle1.position, menuCam.waypoints[index].handle2.position, Color.red, Texture2D.whiteTexture, 1f);
		}
		
	}
}