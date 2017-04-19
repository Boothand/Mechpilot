using System.Collections;
using UnityEngine;

[System.Serializable]
public class CameraOffset
{
	public Vector3 posOffset;
	public Vector3 dirOffset;
}
public class CameraFollow : MechComponent
{
	[SerializeField] Transform target;

	//The time to switch between presets
	[SerializeField] float switchTime = 0.5f;

	[SerializeField] CameraOffset behind, left, right, firstperson;
	CameraOffset currentOffset;
	CameraOffset prevOffset;
	bool switching;

	protected override void OnAwake()
	{
		base.OnAwake();
		currentOffset = right;
	}

	void Start()
	{
		lockOn.OnLockOn += OnLockOn;
		lockOn.OnLockOff += OnLockOff;
	}

	void OnLockOn()
	{
		currentOffset = right;
	}

	void OnLockOff()
	{
		currentOffset = behind;
	}

	IEnumerator SwitchRoutine()
	{
		switching = true;

		Vector3 fromPos = transform.position;
		Vector3 fromForward = transform.forward;

		//Store a reference to the target transform, so it persists through the routine,
		//even if the target transform is changed on the outside.
		CameraOffset toOffset = currentOffset;

		//Store this so it can be compared next time you switch camera preset.
		prevOffset = currentOffset;

		float timer = 0f;

		//Interpolate position and forward axis to the targets
		while (timer < switchTime)
		{
			timer += Time.deltaTime;
			float smoothTime = Mathf.SmoothStep(0f, 1f, timer / switchTime);
			transform.position = Vector3.Lerp(fromPos, TargetPlusOffset(toOffset.posOffset), smoothTime);
			transform.forward = Vector3.Lerp(fromForward, TargetPlusOffset(toOffset.dirOffset) - transform.position, smoothTime);

			//Since we are calling from LateUpdate:
			yield return new WaitForEndOfFrame();
		}

		switching = false;
	}

	Vector3 TargetPlusOffset(Vector3 offset)
	{
		//Returns the target position plus an offset to the target transform,
		//relative to the mech transform
		return target.position + mech.transform.TransformDirection(offset);
	}

	void LateUpdate()
	{
		//Switch camera preset depending on keypress
		if (input.camLeft)
		{
			currentOffset = left;
		}
		else if (input.camRight)
		{
			currentOffset = right;
		}
		else if (input.camBehind)
		{
			currentOffset = behind;
		}
		else if (input.camFP)
		{
			currentOffset = firstperson;
		}

		//If not already switching preset,
		//and the preset is different from the last:
		if (!switching
			&& prevOffset != currentOffset)
		{
			//Start switching
			StopAllCoroutines();
			StartCoroutine(SwitchRoutine());
		}

		//When not switching, set the position and rotation to the target preset.
		if (!switching)
		{
			if (lockOn.lockedOn)
			{
				Vector3 targetPos = TargetPlusOffset(currentOffset.posOffset);

				transform.position = targetPos;
				transform.forward = TargetPlusOffset(currentOffset.dirOffset) - transform.position;
			}
			else
			{
				transform.position = TargetPlusOffset(currentOffset.posOffset);
				transform.forward = TargetPlusOffset(currentOffset.dirOffset) - transform.position;
			}
		}
	}
}