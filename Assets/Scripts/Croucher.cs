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
		pilot.move.ProcessVelocity += WalkSlower;
	}

	void WalkSlower(ref Vector3 velocity)
	{
		if (crouching)
		{
			velocity *= 0.85f;
		}
	}

	void Update()
	{
		float crouchInput = Mathf.Clamp01(input.crouchAxis);

		animCrouchHeight = Mathf.MoveTowards(animCrouchHeight, crouchInput, Time.deltaTime * animCrouchSpeed);
		animator.SetFloat("Crouch", animCrouchHeight);

		actualCrouchHeight = Mathf.MoveTowards(actualCrouchHeight, crouchHeight * crouchInput, Time.deltaTime * crouchSpeed);

		//Vector3 offset = Vector3.down * actualCrouchHeight;

		//arms.OffsetIKTargets(offset, crouchSpeed);
	}
}