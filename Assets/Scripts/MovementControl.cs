//using System.Collections;
using UnityEngine;

public class MovementControl : MonoBehaviour
{
	Animator anim;
	Mech mech;

	[Header("References")]
	[SerializeField]
	Transform head;

	[SerializeField]
	Transform aimBase;

	[Header("Values")]
	[SerializeField]
	float walkSpeed = 5f;

	[SerializeField]
	float accelerationSpeed = 1f;

	[SerializeField]
	string lookX = "Mouse X";
	[SerializeField]
	string lookY = "Mouse Y";

	[SerializeField]
	string moveXStr = "Horizontal";
	[SerializeField]
	string moveYStr = "Vertical";

	Vector3 acceleration;


	void Awake ()
	{
		anim = GetComponent<Animator>();		
		mech = GetComponent<Mech>();
	}
	
	void Update ()
	{
		//Accelerating up and down
		Vector3 inputVec = new Vector3(Input.GetAxisRaw(moveXStr), 0f, Input.GetAxisRaw(moveYStr));

		acceleration = Vector3.MoveTowards(acceleration, inputVec, Time.deltaTime * accelerationSpeed);
		acceleration = Vector3.ClampMagnitude(acceleration, 1f);

		//Looking around
		aimBase.position = head.position;
		aimBase.Rotate(Vector3.up, Input.GetAxis(lookX));
		aimBase.Rotate(head.right, -Input.GetAxis(lookY), Space.World);

		//Which way to walk
		Vector3 headForward = head.forward;
		headForward.y = head.localPosition.y;

		Vector3 headRight = head.right;
		headRight.y = head.localPosition.y;

		if (Mathf.Abs(inputVec.z) > 0.1f)
		{
			transform.forward = Vector3.Lerp(transform.forward, headForward, Time.deltaTime * 2f);
		}

		if (Mathf.Abs(inputVec.x) > 0.1f)
		{
			transform.right = Vector3.Lerp(transform.right, headRight, Time.deltaTime * 2f);
		}

		Debug.DrawRay(head.position, headForward, Color.red);
		Debug.DrawRay(head.position, transform.forward, Color.white * 2f);

		transform.Translate(transform.forward * acceleration.z * walkSpeed * Time.deltaTime, Space.World);
		transform.Translate(transform.right * acceleration.x * walkSpeed * Time.deltaTime, Space.World);

		anim.SetFloat("ForwardMovement", acceleration.z);
		anim.SetFloat("SideMovement", acceleration.x);
		anim.SetFloat("MoveSpeed", Mathf.Abs(acceleration.magnitude));
	}
}