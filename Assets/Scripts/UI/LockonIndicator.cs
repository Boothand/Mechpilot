//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LockonIndicator : MechComponent
{
	[SerializeField] Image indicatorImg;
	[SerializeField] float verticalOffset = 0.5f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		HideIndicator();

		pilot.lockOn.OnGetLockedOn += ShowIndicator;
		pilot.lockOn.OnGetLockedOff += HideIndicator;
	}

	void ShowIndicator()
	{
		if (mech.tempEnemy)
		{
			indicatorImg.enabled = true;
		}
	}

	void HideIndicator()
	{
		indicatorImg.enabled = false;
	}

	void Update()
	{
		//Stay above head
		indicatorImg.transform.position = hierarchy.head.position + Vector3.up * verticalOffset;

		//Face the opponent's camera
		if (mech.tempEnemy &&
			indicatorImg.enabled)
		{
			transform.forward = mech.tempEnemy.pilot.cameraFollow.transform.position - transform.position;
		}
	}
}