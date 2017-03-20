using System.Collections;
using UnityEngine;

public class CameraPositionSwitch : MechComponent
{
	[SerializeField] Transform behind, left, right, firstperson;
	Transform targetPos;
	Transform prevTargetPos;
	[SerializeField] float switchTime = 0.5f;
	bool switching;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start ()
	{
		targetPos = behind;
	}

	IEnumerator SwitchRoutine()
	{
		float timer = 0f;
		Vector3 fromPos = transform.position;
		Quaternion fromRot = transform.rotation;
		Vector3 toPos = targetPos.position;
		Quaternion toRot = targetPos.rotation;
		prevTargetPos = targetPos;

		while (timer < switchTime)
		{
			timer += Time.deltaTime;
			float smoothTime = Mathf.SmoothStep(0f, 1f, timer / switchTime);
			transform.position = Vector3.Lerp(fromPos, toPos, smoothTime);
			transform.rotation = Quaternion.Lerp(fromRot, toRot, smoothTime);

			yield return null;
		}

		switching = false;
	}

	void Update()
	{
		if (input.camLeft)
		{
			targetPos = left;
		}
		else if (input.camRight)
		{
			targetPos = right;
		}
		else if (input.camBehind)
		{
			targetPos = behind;
		}
		else if (input.camFP)
		{
			targetPos = firstperson;
		}

		if (!switching
			&& prevTargetPos != targetPos)
		{
			switching = true;
			StopAllCoroutines();
			StartCoroutine(SwitchRoutine());
		}

		if (!switching)
		{
			transform.position = targetPos.position;
			transform.rotation = targetPos.rotation;
		}
	}
}