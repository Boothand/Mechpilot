//using System.Collections;
using UnityEngine;

//Must exist on the same game object as the animator.
//PlayFootstep is called from an AnimationEvent triggered from each walking animation.
public class FootstepSounds : MechComponent
{
	float timeSinceLast;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void PlayFootstep()
	{
		//Make sure the footsteps aren't spammed, this is annoying to listen to.
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