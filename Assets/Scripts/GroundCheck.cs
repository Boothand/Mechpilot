//using System.Collections;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	[SerializeField] LayerMask groundMask;
	public bool grounded { get; private set; }
	
	void Awake()
	{
		
	}
	
	void Update()
	{
		grounded = false;
		Vector3 rayStartPos = transform.position + Vector3.up * 0.1f;
		Ray groundRay = new Ray(rayStartPos, Vector3.down);
		RaycastHit rayHit;
		Physics.Raycast(groundRay, out rayHit, 0.2f, groundMask);

		if (rayHit.transform)
		{
			grounded = true;
		}

		//Debug.DrawRay(rayStartPos, Vector3.down * 0.2f, Color.red);
	}
}