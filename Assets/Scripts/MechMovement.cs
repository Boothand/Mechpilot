//using System.Collections;
using UnityEngine;

public class MechMovement : MechComponent
{
	CapsuleCollider capsuleCol;

	//Physic materials to prevent sliding
	[SerializeField] PhysicMaterial physMat_stillStanding;
	[SerializeField] PhysicMaterial physMat_moving;

	//Direction, velocity, movement
	public Vector3 inputVec { get; private set; }
	public float inputVecMagnitude { get; private set; }
	Vector3 worldMoveDir;
	public Vector3 getWorldMoveDir { get { return worldMoveDir; } }
	Vector3 velocity;
	Vector3 lastPos;

	public Vector3 getVelocity { get { return velocity; } }

	//Animation
	float animForward, animSide;    //Interpolating values, sent to animator

	[Header("Values")]
	[SerializeField] float moveSpeed = 25f;
	[SerializeField] float maxSlopeAngle = 45f;
	[SerializeField] float accelerationSpeed = 0.5f;
	[SerializeField] float animationSpeedFactor = 0.4f;

	//Flags
	public bool moving { get; private set; }
	public bool running { get; private set; }
	public bool grounded { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		capsuleCol = mech.GetComponent<CapsuleCollider>();
	}

	void OnCollisionStay()
	{
		grounded = true;
	}

	void OnCollisionExit()
	{
		grounded = false;
	}

	Vector3 BuildVelocity() //Return value just for increased readability in the Update-loop
	{
		float minimumInputSpeed = 0.2f;
		float moveHorz = input.moveHorz;
		float moveVert = input.moveVert;

		if (Mathf.Abs(input.moveHorz) > 0.001f &&
			Mathf.Abs(input.moveHorz) < minimumInputSpeed)
		{
			moveHorz = Mathf.Sign(moveHorz) * minimumInputSpeed;
		}

		if (Mathf.Abs(input.moveVert) > 0.001f &&
			Mathf.Abs(input.moveVert) < minimumInputSpeed)
		{
			moveVert = Mathf.Sign(moveVert) * minimumInputSpeed;
		}
		
		inputVec = new Vector3(moveHorz, 0f, moveVert);
		inputVecMagnitude = inputVec.magnitude;

		//Transform direction from 'input' space to world space, relative to head's orientation.
		worldMoveDir = mech.transform.TransformDirection(inputVec);

		//Keep the vector straight regardless of looking up/down
		worldMoveDir.y = 0f;

		//Don't go faster diagonally
		worldMoveDir.Normalize();

		//Scale move speed with the joystick axis
		worldMoveDir *= inputVec.magnitude;

		#region Debug
		//Debug.DrawRay(hierarchy.head.position, worldMoveDir * scaleFactor);


		//if (true)//rb.velocity.y > 0.001f)
		//{
		//	Vector3 rayStartPos = mech.transform.position + Vector3.up * 0.02f * scaleFactor;
		//	Vector3 dir = Vector3.down;
		//	Ray ray = new Ray(rayStartPos, dir);
		//	Debug.DrawRay(rayStartPos, dir * 0.16f * scaleFactor, Color.blue);
		//	RaycastHit hitInfo;
		//	Physics.Raycast(ray, out hitInfo, 0.16f * scaleFactor);

		//	if (hitInfo.transform)
		//	{
		//		float dot = Vector3.Dot(Vector3.up, hitInfo.normal);

		//		if (dot > 0.7f)
		//		{
		//			//worldMoveDir = Vector3.zero;
		//			//velocity = Vector3.zero;
		//			rb.velocity = Vector3.zero;
		//		}
		//	}
		//}
		#endregion

		float accelerationSpeedToUse = accelerationSpeed;

		//Send velocity to dash component for potential modification
		dasher.RunComponent(ref velocity, ref accelerationSpeedToUse);

		//Send velocity to run function for potential modification
		CheckRun(ref worldMoveDir);

		dodger.DodgeVelocityModification(ref velocity);

		if (running)
		{
			accelerationSpeedToUse *= 5f;
		}

		//Move velocity towards the desired direction, with a set acceleration
		Vector3 vel = Vector3.MoveTowards(velocity, worldMoveDir, Time.deltaTime * accelerationSpeedToUse);

		//vel = Vector3.ClampMagnitude(vel, 1f);
		return vel;
	}

	void CheckRun(ref Vector3 velocity)
	{
		running = false;

		if (input.run > 0.3f &&
			input.moveVert > 0.2f &&
			Mathf.Abs(input.moveHorz) < 0.3f)
		{
			//Plan: Ta en viss start-stamina for å begynde å springe.
			//Sakte akselerasjon og deselerasjon
			running = true;

			velocity *= 2.5f * input.run;

			float staminaAmount = velocity.magnitude * Time.deltaTime * 10f;

			energyManager.SpendStamina(staminaAmount);
		}

		float animSpeed = rb.velocity.magnitude / 60;
		//print(animSpeed);

		animator.SetBool("Running", running);
		animator.SetFloat("RunAmount", animSpeed);
	}

	PhysicMaterial GetAppropriatePhysicMaterial()
	{
		if (moving)
		{
			return physMat_moving;
		}

		return physMat_stillStanding;
	}

	void DoWalkAnimation()
	{
		float blendSpeed = 2f;

		//Transform world velocity to local space, to map forward and side values in animator
		Vector3 animationVector = mech.transform.InverseTransformDirection(velocity);
		//Normalize, so blend tree is mapped correctly
		animationVector.Normalize();

		//Blend smoothly to input directions
		animForward = Mathf.MoveTowards(animForward, animationVector.z, Time.deltaTime * blendSpeed);
		animSide = Mathf.MoveTowards(animSide, animationVector.x, Time.deltaTime * blendSpeed);

		
		animator.SetFloat("ForwardMovement", animForward);
		animator.SetFloat("SideMovement", animSide);

		//Animation speed follows actual mech speed
		float animSpeed = 1f;

		//Vector3 actualVelocity = mech.transform.position - lastPos;
		if (moving)
		{
			animSpeed = rb.velocity.magnitude / scaleFactor * animationSpeedFactor;
		}
		animator.SetFloat("MoveSpeed", animSpeed);

		lastPos = mech.transform.position;
	}

	void FixedUpdate()
	{
		//------------------ MOVING ------------------\\
		//Apply the velocity on X and Z axis, and regular y velocity (gravity) from rigidbody.

		Vector3 gravityVector = Vector3.up * rb.velocity.y;

		//Depending on base move speed and available energy
		Vector3 moveVectorXZ = velocity * moveSpeed * scaleFactor * Time.deltaTime;
		moveVectorXZ.y = 0f;

		//mech.transform.position += moveVectorXZ * 0.02f;
		//rb.MovePosition(mech.transform.position + moveVectorXZ * 0.02f);
		gravityVector.y = Mathf.Clamp(gravityVector.y, -10f, 0f);
		rb.velocity = new Vector3(moveVectorXZ.x, rb.velocity.y, moveVectorXZ.z);
		//rb.velocity = moveVectorXZ + gravityVector;

		//rb.AddForce(moveVectorXZ);
	}

	void Update()
	{
		//------------------ MOVING ------------------\\

		//Gradual velocity build-up
		velocity = BuildVelocity();

		//You are moving if velocity is greater than 0
		//moving = rb.velocity.magnitude > 0.001f;
		moving = velocity.magnitude > 0.001f;


		//Prevent unwanted sliding by switching out the physic material
		//capsuleCol.material = GetAppropriatePhysicMaterial();

		//Walk animation
		DoWalkAnimation();

		//Gradually tween mech's forward direction towards head's aim direction
		if (true)//inputVec.magnitude > 0.01f && moving)
		{
			mech.transform.forward = Vector3.Lerp(mech.transform.forward, pilot.headRotation.getForwardDir, Time.deltaTime * 2f);
		}
	}
}