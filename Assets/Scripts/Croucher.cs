//using System.Collections;
using UnityEngine;

public class Croucher : MechComponent
{
	public float crouchHeight { get; private set; }
	[SerializeField] float crouchSpeed = 1.3f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Update()
	{
		float crouchInput = Mathf.Clamp01(input.crouchAxis);

		crouchHeight = Mathf.MoveTowards(crouchHeight, crouchInput, Time.deltaTime * crouchSpeed);

		animator.SetFloat("Crouch", crouchHeight);
	}
}