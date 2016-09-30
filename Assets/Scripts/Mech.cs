using UnityEngine;

public class Mech : MonoBehaviour
{
	Animator anim;
	float walkMovement;
	Rigidbody rb;
	[SerializeField]
	float walkSpeed = 10f;
	[SerializeField]
	float accelerationSpeed = 1f;

	float acceleration;

	void Start ()
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
	{
		//Movement ultra prototype
		Vector3 inputVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

		if (inputVec.z > 0.1f)
		{
			acceleration += accelerationSpeed;
		}
		else
		{
			acceleration -= accelerationSpeed;
		}

		acceleration = Mathf.Clamp(acceleration, 0f, 1f);



		print(acceleration);

		Vector3 moveVec = transform.forward * acceleration;
		transform.Translate(moveVec * walkSpeed * Time.deltaTime);
	}
}