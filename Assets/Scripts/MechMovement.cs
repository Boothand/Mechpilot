//using System.Collections;
using UnityEngine;

//Makes the rigidbody apply a velocity every fixed update,
//Makes sure other components get a chance to modify the velocity
//before it is applied by using events and referencing.
public class MechMovement : MechComponent
{
	//CapsuleCollider capsuleCol;

	//Movement input, mapped to a vector.
	public Vector3 inputVec { get; private set; }

	//Calculate the magnitude once, no need to compute it every time.
	public float inputVecMagnitude { get; private set; }

	//Normalized direction we're going in, in world space. Use inputVec for local space.
	Vector3 worldMoveDir;
	public Vector3 getWorldMoveDir { get { return worldMoveDir; } }

	//The velocity that is applied on X and Z in world space.
	Vector3 velocity;
	public Vector3 getVelocity { get { return velocity; } }

	public Vector3 localVelocity { get; private set; }

	//Calculate the magnitude once, no need to compute it every time.
	public float velocityMagnitude { get; private set; }

	CapsuleCollider capsuleCol;

	//Animation
	float animForward, animSide;    //Interpolating values, sent to animator

	[Header("Values")]
	[SerializeField] float moveSpeed = 25f;	//Movement multiplier
	//[SerializeField] float maxSlopeAngle = 45f;
	[SerializeField] float accelerationSpeed = 0.5f;	//How fast to start and stop walking/running
	[SerializeField] float animationSpeedFactor = 0.4f; //Movement animation speed multiplier

	//Blend speed for updating the animator's blend tree.
	[SerializeField] float animBlendSpeed = 5f;

	public bool moving { get; private set; }

	//Sends a reference to a vector, so not a copy is changed, but the actual vector.
	public delegate void VectorReference(ref Vector3 vec);
	public delegate void VectorFloatReference(ref Vector3 vec, ref float num);
	public event VectorReference ProcessVelocity;
	public event VectorReference ProcessWorldMoveDir;
	public event VectorFloatReference ProcessVelAndAcc;

	protected override void OnAwake()
	{
		base.OnAwake();

		capsuleCol = mech.GetComponent<CapsuleCollider>();
	}

	Vector3 BuildVelocity() //Return value just for increased readability in the Update-loop
	{
		float moveHorz = input.moveHorz;
		float moveVert = input.moveVert;
		float minimumInputSpeed = 0.2f;	//Walking slower than this is pointless.

		//Don't walk super slow.
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

		//Transform direction from 'input' space to world space, relative to mech's orientation.
		worldMoveDir = mech.transform.TransformDirection(inputVec);

		//Keep the movement vector straight regardless of looking up/down.
		worldMoveDir.y = 0f;

		//Don't go faster diagonally.
		worldMoveDir.Normalize();

		//Scale move speed with the joystick axis.
		worldMoveDir *= inputVecMagnitude;

		float accelerationSpeedToUse = accelerationSpeed;

		//Send velocity to dash component for potential modification
		//pilot.dasher.ModifyVelAndAcc(ref velocity, ref accelerationSpeedToUse);
		if (ProcessVelAndAcc != null)
			ProcessVelAndAcc(ref velocity, ref accelerationSpeedToUse);

		//Run event for anyone to use to modify worldMoveDir before it is applied.
		if (ProcessWorldMoveDir != null)
			ProcessWorldMoveDir(ref worldMoveDir);

		//Run event for anyone to use to modify velocity before it is applied.
		if (ProcessVelocity != null)
			ProcessVelocity(ref velocity);

		if (pilot.run.running)
		{
			accelerationSpeedToUse *= 5f;
		}

		//Move velocity towards the desired direction, with a set acceleration
		Vector3 finalVelocity = Vector3.MoveTowards(velocity, worldMoveDir, Time.deltaTime * accelerationSpeedToUse);

		velocityMagnitude = finalVelocity.magnitude;

		//vel = Vector3.ClampMagnitude(vel, 1f);
		return finalVelocity;
	}

	void DoWalkAnimation()
	{
		//Blend smoothly to input directions
		animForward = Mathf.MoveTowards(animForward, localVelocity.z, Time.deltaTime * animBlendSpeed);
		animSide = Mathf.MoveTowards(animSide, localVelocity.x, Time.deltaTime * animBlendSpeed);
		
		//Map to blend tree in animator.
		animator.SetFloat("ForwardMovement", animForward);
		animator.SetFloat("SideMovement", animSide);

		//Animation speed follows actual mech speed
		float animSpeed = 1f;
		
		//Sync the animation movement speed to the animator.
		if (moving)
		{
			animSpeed = rb.velocity.magnitude / scaleFactor * animationSpeedFactor;
		}

		animator.SetFloat("MoveSpeed", animSpeed);
	}

	protected override void OnFixedUpdate()
	{
		//Apply the velocity on X and Z axis, and regular y velocity (gravity) from rigidbody.
		Vector3 moveVectorXZ = velocity * moveSpeed * scaleFactor * Time.deltaTime;
		moveVectorXZ.y = 0f;	//Probably not necessary, but cancel out any y motion.
		
		rb.velocity = new Vector3(moveVectorXZ.x, rb.velocity.y, moveVectorXZ.z);
	}

	protected override void OnUpdate()
	{
		//Gradual velocity build-up
		velocity = BuildVelocity();

		//You are moving if velocity is greater than 0
		moving = velocityMagnitude > 0.001f;

		//Transform world velocity to local space, so others can get it (including this class).
		localVelocity = mech.transform.InverseTransformDirection(velocity);
		localVelocity.Normalize();

		//Walk animation
		DoWalkAnimation();
	}
}