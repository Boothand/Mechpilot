using System.Collections;
using UnityEngine;

public class Jumper : MechComponent
{
	[SerializeField] float jumpForce = 1f;
	public bool jumping { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{

	}

	IEnumerator JumpRoutine()
	{
		jumping = true;
		animator.CrossFadeInFixedTime("Jump", 0.1f);

		yield return new WaitForSeconds(0.35f);
		rb.velocity += Vector3.up * jumpForce;

		yield return new WaitForSeconds(0.6f);
		animator.CrossFadeInFixedTime("Land", 0.1f);


		jumping = false;
	}

	void Update()
	{
		if (!jumping &&
			input.jump
			//&& pilot.move.grounded
			)
		{
			StartCoroutine(JumpRoutine());
		}
	}
}