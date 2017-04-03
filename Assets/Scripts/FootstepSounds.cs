//using System.Collections;
using UnityEngine;

public class FootstepSounds : MechComponent
{
	float timeSinceLast;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void PlayFootstep()
	{
		if (timeSinceLast > 0.3f)
		{
			mechSounds.PlayFootStepSound();
			timeSinceLast = 0f;
		}
	}

	void Update()
	{
		timeSinceLast += Time.deltaTime;
	}
}