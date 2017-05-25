using System.Collections;
using UnityEngine;

public class VictoryCamera : MonoBehaviour
{
	public static VictoryCamera instance;
	Transform target;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		gameObject.SetActive(false);
	}

	public void LookAt(Mech player)
	{
		transform.position = player.transform.position +
							 player.transform.forward * 2f +
							 Vector3.up;

		transform.forward = (player.transform.position + Vector3.up * 1.5f) - transform.position;

		target = player.transform;
	}

	public void LookAt2(Mech player)
	{
		transform.position = player.transform.position +
							 player.transform.forward * 4f +
							 Vector3.up * 10;

		transform.forward = (player.transform.position + Vector3.up) - transform.position;

		target = player.transform;
	}

	void Update()
	{
		if (target != null)
		{
			transform.position = Vector3.Lerp(transform.position, target.transform.position +
																  target.transform.forward * 2f +
																  Vector3.up, Time.deltaTime * 0.7f);

			transform.forward = Vector3.Lerp(transform.forward, (target.position + Vector3.up * 1.5f) -
																 transform.position,
																 Time.deltaTime * 0.7f);
		}
	}
}