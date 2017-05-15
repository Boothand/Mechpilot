using System.Collections;
using UnityEngine;

[System.Serializable]
public class CameraWaypoint
{
	public Transform waypoint;
	public Transform handle1, handle2;
	public float time = 5f;
}

public class MenuCameraRail : MonoBehaviour
{
	[SerializeField] public CameraWaypoint[] waypoints;

	void Awake()
	{
		
	}

	void Start()
	{
		StartCoroutine(GoToWaypointRoutine());
	}

	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float u = 1f - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0; //first term
		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3; //fourth term

		return p;
	}

	IEnumerator GoToWaypointRoutine()
	{
		int waypointIndex = 0;

		while (true)
		{
			CameraWaypoint activeWaypoint = waypoints[waypointIndex];
			waypointIndex++;

			if (waypointIndex == waypoints.Length)
				waypointIndex = 0;

			CameraWaypoint nextWaypoint = waypoints[waypointIndex];

			Vector3 targetPos = nextWaypoint.waypoint.position;
			Vector3 startPos = activeWaypoint.waypoint.position;
			Quaternion startRot = transform.rotation;
			Quaternion targetRot = nextWaypoint.waypoint.rotation;

			float duration = activeWaypoint.time;

			float timer = 0f;

			Vector3 handle1 = activeWaypoint.handle1.position;
			Vector3 handle2 = activeWaypoint.handle2.position;

			while (timer < duration)
			{
				timer += Time.deltaTime;
				transform.position = CalculateBezierPoint(timer / duration, startPos, handle1, handle2, targetPos);
				transform.rotation = Quaternion.Lerp(startRot, targetRot, timer / duration);

				yield return null;
			}
		}
	}
}