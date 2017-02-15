//using System.Collections;
using UnityEngine;

public class ElbowTargetFix : MechComponent
{
	[SerializeField] Transform elbowL, elbowR;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		elbowR.position = hierarchy.rhand.position - hierarchy.rhand.forward * 100f;
		elbowL.position = hierarchy.lHand.position - hierarchy.lHand.forward * 100f;

		elbowR.position = new Vector3(elbowR.position.x, hierarchy.rhand.position.y - 100f, elbowR.position.z);
		elbowL.position = new Vector3(elbowL.position.x, hierarchy.lHand.position.y - 100f, elbowL.position.z);
		Debug.DrawLine(hierarchy.rhand.position, elbowR.position, Color.red);
	}
}