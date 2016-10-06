using UnityEngine;

public class Mech : MonoBehaviour
{
	Animator anim;
	Rigidbody rb;

	float walkMovement;

	[SerializeField]
	float walkSpeed = 5f;
	[SerializeField]
	float accelerationSpeed = 0.01f;

	float accelerationZ;
	Vector3 acceleration;

	[SerializeField]
	Transform head;

	[SerializeField]
	Transform aimBase;

	void Start ()
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	
	void Update ()
	{
		//Accelerating up and down
		Vector3 inputVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

		accelerationZ = Mathf.MoveTowards(accelerationZ, inputVec.z, Time.deltaTime * accelerationSpeed);
		accelerationZ = Mathf.Clamp(accelerationZ, -1f, 1f);

		acceleration = Vector3.MoveTowards(acceleration, inputVec, Time.deltaTime * accelerationSpeed);
		acceleration = Vector3.ClampMagnitude(acceleration, 1f);

		//Looking around
		aimBase.position = head.position;
		aimBase.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
		//aimBase.Rotate(head.right, -Input.GetAxis("Mouse Y"));

		//Which way to walk
		Vector3 headForward = head.forward;
		headForward.y = transform.position.y;

		Vector3 headRight = head.right;
		headRight.y = transform.position.y;

		if (Mathf.Abs(inputVec.z) > 0.1f)
		{
			transform.forward = Vector3.Lerp(transform.forward, headForward, Time.deltaTime * 2f);
		}

		if (Mathf.Abs(inputVec.x) > 0.1f)
		{
			transform.right = Vector3.Lerp(transform.right, headRight, Time.deltaTime * 2f);
		}

		Debug.DrawRay(head.position, head.forward, Color.red);
		Debug.DrawRay(head.position, transform.forward, Color.white * 2f);

		
		transform.Translate(transform.forward * acceleration.z * Time.deltaTime, Space.World);
		transform.Translate(transform.right * acceleration.x * Time.deltaTime, Space.World);

		anim.SetFloat("ForwardMovement", acceleration.z);
		anim.SetFloat("SideMovement", acceleration.x);
		anim.SetFloat("MoveSpeed", Mathf.Abs(acceleration.magnitude));
	}
}