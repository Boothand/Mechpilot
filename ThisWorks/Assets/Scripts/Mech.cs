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

		//Looking around
		aimBase.position = head.position;
		aimBase.Rotate(Vector3.up, Input.GetAxis("Mouse X"));

		//Which way to walk
		Vector3 dirToWalk = head.forward;
		dirToWalk.y = transform.position.y;

		if (Mathf.Abs(inputVec.z) > 0.1f)
		{
			transform.forward = Vector3.Lerp(transform.forward, dirToWalk, Time.deltaTime * 2f);
		}
		Debug.DrawRay(head.position, head.forward, Color.red);
		Debug.DrawRay(head.position, transform.forward, Color.white * 2f);


		Vector3 moveVec = transform.forward * accelerationZ;
		transform.Translate(transform.forward * accelerationZ * Time.deltaTime, Space.World);
		transform.Translate(transform.right * inputVec.x * Time.deltaTime, Space.World);

		anim.SetFloat("Movement", accelerationZ);
		anim.SetFloat("MoveSpeed", Mathf.Abs(accelerationZ));
	}
}