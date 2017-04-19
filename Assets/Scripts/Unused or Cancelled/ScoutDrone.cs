//using System.Collections;
using UnityEngine;

public class ScoutDrone : MechComponent
{
	Rigidbody rbody;
	Vector3 slidyForward;
	float velocity;
	float rotVelocity;
	[SerializeField] float speed = 3f;

	protected override void OnAwake()
	{
		base.OnAwake();
		rbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		//Turning
		float side = Mathf.Clamp(input.droneSide, -1, 1);
		rotVelocity = Mathf.MoveTowards(rotVelocity, side, Time.deltaTime * 15f);
		
		transform.Rotate(Vector3.up, rotVelocity * Time.deltaTime * velocity * 50);

		//Acceleration
		float forward = Mathf.Clamp(input.droneDrive, -1, 1);
		velocity = Mathf.MoveTowards(velocity, forward * speed, Time.deltaTime * 3f);

		//Change forward/back direction abruptly, faster
		if (velocity > 0 && forward < 0 ||
			velocity < 0 && forward > 0)
		{
			velocity += forward * 0.1f;
		}

		//If you wanna powerslide...
		float steadyFactor = 3f;

		if (input.dronePowerslide)
		{
			steadyFactor = 1f;
		}

		slidyForward = Vector3.MoveTowards(slidyForward, transform.forward, Time.deltaTime * steadyFactor);
	}

	void FixedUpdate()
	{
		//Apply velocity
		Vector3 velocityZ = slidyForward * velocity;
		rbody.velocity = new Vector3(velocityZ.x, rbody.velocity.y, velocityZ.z);
	}
}