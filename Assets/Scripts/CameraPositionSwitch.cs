//using System.Collections;
using UnityEngine;

public class CameraPositionSwitch : MechComponent
{
	[SerializeField] Transform behind, left, right, firstperson;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start ()
	{
		transform.SetParent(behind);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	void Update()
	{
		if (input.camLeft)
		{
			transform.SetParent(left);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		else if (input.camRight)
		{
			transform.SetParent(right);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		else if (input.camBehind)
		{
			transform.SetParent(behind);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		else if (input.camFP)
		{
			transform.SetParent(firstperson);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}