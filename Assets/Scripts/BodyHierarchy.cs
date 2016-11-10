//using System.Collections;
using UnityEngine;

public class BodyHierarchy : MonoBehaviour
{
	[SerializeField]
	Transform head;

	[SerializeField]
	Transform rShoulder, lShoulder;

	[SerializeField]
	Transform rElbow, lElbow;

	[SerializeField]
	Transform rhand, lHand;

	[SerializeField]
	Transform rThigh, lThigh;

	[SerializeField]
	Transform rKnee, lKnee;

	[SerializeField]
	Transform rFoot, lFoot;

	public Vector3 headPos { get { return head.position; } }
	public Vector3 rShoulderPos { get { return rShoulder.position; } }
	public Vector3 lShoulderPos { get { return lShoulder.position; } }
	public Vector3 rElbowPos { get { return rElbow.position; } }
	public Vector3 lElbowPos { get { return lElbow.position; } }
	public Vector3 rhandPos { get { return rhand.position; } }
	public Vector3 lHandPos { get { return lHand.position; } }
	public Vector3 rThighPos { get { return rThigh.position; } }
	public Vector3 lThighPos { get { return lThigh.position; } }
	public Vector3 rKneePos { get { return rKnee.position; } }
	public Vector3 lKneePos { get { return lKnee.position; } }
	public Vector3 rFootPos { get { return rFoot.position; } }
	public Vector3 lFootPos { get { return lFoot.position; } }
}