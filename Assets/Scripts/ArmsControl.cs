//using System.Collections;
using UnityEngine;
using RootMotion.FinalIK;

public class ArmsControl : MonoBehaviour
{
	Mech mech;
	FullBodyBipedIK ik;

	[SerializeField]
	Transform rHandTarget, lHandTarget;

	void Start ()
	{
		mech = GetComponent<Mech>();
		ik = GetComponent<FullBodyBipedIK>();
	}
	
	void Update ()
	{
		rHandTarget.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f) * Time.deltaTime;
		lHandTarget.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * Time.deltaTime;


		IKEffector rHand = ik.solver.rightHandEffector;
		IKEffector lHand = ik.solver.leftHandEffector;

		rHand.position = rHandTarget.position;
		lHand.position = lHandTarget.position;
		rHand.positionWeight = 1f;
		lHand.positionWeight = 1f;

	}
}