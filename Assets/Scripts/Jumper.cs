using System.Collections;
using UnityEngine;

public class Jumper : MechComponent
{
	[SerializeField] float jumpForce = 1f;
	public bool jumping { get; private set; }
	[SerializeField] float staminaUsage = 20f;

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
		energyManager.SpendStamina(staminaUsage);

		yield return new WaitForSeconds(0.3f);
		while (!groundCheck.grounded)
		{
			yield return null;
		}
		animator.CrossFadeInFixedTime("Land", 0.1f);

		jumping = false;
	}

	void Update()
	{
		if (!jumping &&
			input.jump
			&& groundCheck.grounded
			&& energyManager.CanSpendStamina(staminaUsage)
			)
		{
			StartCoroutine(JumpRoutine());
		}
	}
}