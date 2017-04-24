//using System.Collections;
using UnityEngine;

//Very simple check to see if there is ground directly underneath us.
public class GroundCheck : MonoBehaviour
{
	[SerializeField] LayerMask groundMask;	//Tell us what layers are ground.
	public bool grounded { get; private set; }
	
	void Awake()
	{
		
	}
	
	void Update()
	{
		grounded = false;

		Vector3 rayStartPos = transform.position + Vector3.up * 0.1f;	//Start slightly above feet.
		Ray groundRay = new Ray(rayStartPos, Vector3.down);
		RaycastHit rayHit;

		//Raycast slightly below feet.
		Physics.Raycast(groundRay, out rayHit, 0.2f, groundMask);

		if (rayHit.transform)
		{
			grounded = true;
		}
	}
}