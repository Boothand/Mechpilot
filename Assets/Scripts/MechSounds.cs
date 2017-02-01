﻿//using System.Collections;
using UnityEngine;

public class MechSounds : MechComponent
{
	AudioSource singleSource;
	[SerializeField] AudioSource loopSource;
	[SerializeField] AudioClip moveArmBeginSound;
	[SerializeField] AudioClip moveArmSound;
	[SerializeField] AudioClip moveArmEndSound;

	protected override void OnAwake()
	{
		base.OnAwake();
		singleSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		arms.armControl.OnMoveArmBegin -= PlayOnMoveBeginSound;
		arms.armControl.OnMoveArmBegin += PlayOnMoveBeginSound;
		arms.armControl.OnMoveArm -= PlayOnMoveSound;
		arms.armControl.OnMoveArm += PlayOnMoveSound;
		arms.armControl.OnMoveArmEnd -= PlayOnMoveEndSound;
		arms.armControl.OnMoveArmEnd += PlayOnMoveEndSound;
	}

	void PlayOnMoveBeginSound()
	{
		singleSource.clip = moveArmBeginSound;
		if (!singleSource.isPlaying)
		{
			singleSource.PlayOneShot(moveArmBeginSound);
		}
		//loopSource.volume = 1f;
		loopSource.loop = true;
		loopSource.clip = moveArmSound;
		loopSource.Play();
	}

	void PlayOnMoveSound()
	{
		
	}

	void PlayOnMoveEndSound()
	{
		if (singleSource.clip != moveArmEndSound)
		{
			singleSource.clip = moveArmEndSound;
			singleSource.PlayOneShot(moveArmEndSound);
			//loopSource.volume = 0f;
		}

		loopSource.Stop();
	}

	void Update()
	{
		
	}
}