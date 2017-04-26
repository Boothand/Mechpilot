//using System.Collections;
using UnityEngine;

public class Croucher : MechComponent
{
	public float animCrouchHeight { get; private set; }
	[SerializeField] float animCrouchSpeed = 1.3f;
	[SerializeField] float crouchSpeed = 20f;
	[SerializeField] float crouchHeight = 0.5f;
	float actualCrouchHeight;

	public bool crouching
	{ get { return animCrouchHeight > 0.5f; } }


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		//Modify velocity before it is applied
		pilot.move.ProcessWorldMoveDir += WalkSlower;
	}

	void WalkSlower(ref Vector3 velocity)
	{
		if (crouching)
		{
			velocity *= 0.7f;
		}
	}

	//Updates the animator's blend tree with a crouch variable, controlled by input.
	protected override void OnUpdate()
	{
		float crouchInput = Mathf.Clamp01(input.crouchAxis);	//0 - stand, 1 - crouch

		//Tween the crouch height
		animCrouchHeight = Mathf.MoveTowards(animCrouchHeight, crouchInput, Time.deltaTime * animCrouchSpeed);
		animator.SetFloat("Crouch", animCrouchHeight);
	}
}