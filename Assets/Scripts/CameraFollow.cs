using System.Collections;
using UnityEngine;

[System.Serializable]
public class CameraPreset
{
	public Vector3 posOffset;
	public Vector3 dirOffset;
}

public class CameraFollow : MechComponent
{
	[SerializeField] Transform target;

	//The time to switch between presets
	[SerializeField] float switchTime = 0.5f;

	[SerializeField] CameraPreset behind, left, right, firstperson;
	CameraPreset currentPreset;	//The current preset
	CameraPreset prevPreset;	//Used for comparison
	bool switching;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		//Init the preset
		if (pilot.lockOn.lockedOn)
			OnLockOn();
		else
			OnLockOff();

		//Get a callback from the lockon component
		pilot.lockOn.OnLockOn += OnLockOn;
		pilot.lockOn.OnLockOff += OnLockOff;
	}

	void OnLockOn()
	{
		//When locked on, the camera should auto-switch to the right preset
		currentPreset = right;
	}

	void OnLockOff()
	{
		//When locked off, the camera should auto-switch to the behind preset
		currentPreset = behind;
	}

	IEnumerator SwitchRoutine()
	{
		switching = true;

		Vector3 fromPos = transform.position;
		Vector3 fromForward = transform.forward;

		//Store a reference to the target transform, so it persists through the routine,
		//even if the target transform is changed on the outside.
		CameraPreset toOffset = currentPreset;

		//Store this so it can be compared next time you switch camera preset.
		prevPreset = currentPreset;

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

	//Update after all other transformations:
	protected override void OnLateUpdate()
	{
		//Switch camera preset depending on keypress
		if (input.camLeft)
		{
			currentPreset = left;
		}
		else if (input.camRight)
		{
			currentPreset = right;
		}
		else if (input.camBehind)
		{
			currentPreset = behind;
		}
		else if (input.camFP)
		{
			currentPreset = firstperson;
		}

		//If not already switching preset,
		//and the preset is different from the last:
		if (!switching
			&& prevPreset != currentPreset)
		{
			//Start switching
			StopAllCoroutines();
			StartCoroutine(SwitchRoutine());
		}

		//When not switching, set the position and rotation to the target preset.
		if (!switching)
		{
			if (pilot.lockOn.lockedOn)
			{
				Vector3 targetPos = TargetPlusOffset(currentPreset.posOffset);

				transform.position = targetPos;
				transform.forward = TargetPlusOffset(currentPreset.dirOffset) - transform.position;
			}
			else
			{
				transform.position = TargetPlusOffset(currentPreset.posOffset);
				transform.forward = TargetPlusOffset(currentPreset.dirOffset) - transform.position;
			}
		}
	}
}