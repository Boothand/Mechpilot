//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//A 'lock on' symbol placed over the head, only visible for the enemy.
public class LockonIndicator : MechComponent
{
	[SerializeField] Image indicatorImg;
	[SerializeField] float verticalOffset = 0.5f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		//Start hidden.
		HideIndicator();

		//Show when locked on, hide when locked off.
		pilot.lockOn.OnGetLockedOn += ShowIndicator;
		pilot.lockOn.OnGetLockedOff += HideIndicator;
	}

	void ShowIndicator()
	{
		indicatorImg.enabled = true;
	}

	void HideIndicator()
	{
		indicatorImg.enabled = false;
	}

	protected override void OnUpdate()
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