//using System.Collections;
using UnityEngine;

//Convenience class to quickly refer to positions on the mech
//or in some cases modify the transform, like rotating a hand.
public class BodyHierarchy : MonoBehaviour
{
	public Transform head, neck;
	public Transform rShoulder, lShoulder;
	public Transform spine;
	public Transform rElbow, lElbow;
	public Transform rhand, lHand;
	public Transform rThigh, lThigh;
	public Transform rKnee, lKnee;
	public Transform rFoot, lFoot;
}