//using System.Collections;
using UnityEngine;

public class CameraFollow : MechComponent
{
	[SerializeField] Transform target;
	[SerializeField] Vector3 posOffset = new Vector3(0f, 5, -10f);
	[SerializeField] Vector3 dirOffset = new Vector3(0f, 0f, 0f);
	[SerializeField] bool useDamp;
	[SerializeField] float cameraDamp = 1f;

	[SerializeField] Vector3 lockonPosOffset, lockonDirOffset;

	Vector3 startPosOffset, startDirOffset;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start ()
	{
		startPosOffset = posOffset;
		startDirOffset = dirOffset;
		//transform.SetParent(null);
	}

	void LateUpdate()
	{
		if (pilot.headRotation.lockedOn)
		{
			if (useDamp)
			{
				Vector3 targetPos = target.position + target.TransformDirection(posOffset + lockonPosOffset);
				transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraDamp * 40f);
				transform.forward = Vector3.Lerp(transform.forward, (target.position + target.TransformDirection(dirOffset) - transform.position).normalized, Time.deltaTime * cameraDamp * 40f);
			}
			else
			{
				Vector3 targetPos = target.position + target.TransformDirection(posOffset + lockonPosOffset);

				transform.position = targetPos;
				transform.forward = (target.position + target.TransformDirection(dirOffset + lockonDirOffset) - transform.position).normalized;
			}
		}
		else
		{
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
}