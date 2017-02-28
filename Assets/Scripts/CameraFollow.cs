//using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] Vector3 posOffset = new Vector3(0f, 5, -10f);
	[SerializeField] Vector3 dirOffset = new Vector3(0f, 0f, 0f);
	[SerializeField] bool useDamp;
	[SerializeField] float cameraDamp = 1f;

	void Start ()
	{
		transform.SetParent(null);
	}

	void LateUpdate()
	{
		Vector3 targetPos = target.position + target.TransformDirection(posOffset);

		if (useDamp)
		{
			transform.position = Vector3.Lerp(transform.position, target.position + target.TransformDirection(posOffset), Time.deltaTime * cameraDamp * 40f);
			transform.forward = Vector3.Lerp(transform.forward, (target.position + target.TransformDirection(dirOffset) - transform.position).normalized, Time.deltaTime * cameraDamp * 40f);
		}
		else
		{
			transform.position = target.position + target.TransformDirection(posOffset);
			transform.forward = (target.position + target.TransformDirection(dirOffset) - transform.position).normalized;
		}
	}
}