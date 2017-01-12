//using System.Collections;
using UnityEngine;

public class MechMovement : MechComponent
{
	CapsuleCollider capsuleCol;

	//Physic materials to prevent sliding
	[SerializeField] PhysicMaterial physMat_stillStanding;
	[SerializeField] PhysicMaterial physMat_moving;

	//Direction, velocity, movement
	Vector3 inputVec;
	Vector3 worldMoveDir;
	Vector3 velocity;

	public Vector3 getVelocity { get { return velocity; } }

	//Animation
	float animForward, animSide;    //Interpolating values, sent to animator

	[Header("Values")]
	[SerializeField] float moveSpeed = 50f;
	[SerializeField] float accelerationSpeed = 0.5f;

	//Flags
	public bool moving { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		capsuleCol = mech.GetComponent<CapsuleCollider>();

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


		//Transform direction from 'input' space to world space, relative to head's orientation.
		worldMoveDir = hierarchy.head.TransformDirection(inputVec);

		//Keep the vector straight regardless of looking up/down
		worldMoveDir.y = 0f;

		//Don't go faster diagonally
		//if (worldMoveDir.magnitude > 1f)
		//{
			worldMoveDir.Normalize();
		worldMoveDir *= inputVec.magnitude;
		//}
		Debug.DrawRay(hierarchy.head.position, worldMoveDir);

		//Move velocity towards the desired direction, with a set acceleration
		Vector3 vel = Vector3.MoveTowards(velocity, worldMoveDir, Time.deltaTime * accelerationSpeed);

		vel = Vector3.ClampMagnitude(vel, 1f);
		return vel;
	}

	PhysicMaterial GetAppropriatePhysicMaterial()
	{
		if (moving)
		{
			//print("OK");
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
		animator.SetFloat("MoveSpeed", rb.velocity.magnitude);
	}

	public void RunComponentFixed()
	{
		//------------------ MOVING ------------------\\
		//Apply the velocity on X and Z axis, and regular y velocity (gravity) from rigidbody.

		Vector3 gravityVector = Vector3.up * rb.velocity.y;

		//Depending on base move speed and available energy
		Vector3 moveVectorXZ = velocity * moveSpeed * engineer.energies[HELM_INDEX] * Time.deltaTime;
		moveVectorXZ.y = 0f;

		rb.velocity = moveVectorXZ + gravityVector;
	}

	public void RunComponent()
	{
		//------------------ MOVING ------------------\\

		//Gradual velocity build-up
		velocity = BuildVelocity();

		//You are moving if velocity is greater than 0
		//moving = rb.velocity.magnitude > 0.001f;
		moving = velocity.magnitude > 0.001f;


		//Prevent unwanted sliding by switching out the physic material
		capsuleCol.material = GetAppropriatePhysicMaterial();

		//Walk animation
		DoWalkAnimation();

		//Gradually tween mech's forward direction towards head's aim direction
		if (moving)
		{
			mech.transform.forward = Vector3.Lerp(mech.transform.forward, helm.headRotation.lookDir, Time.deltaTime * 2f);
		}
	}
}