﻿//using System.Collections;
using UnityEngine;

public class MechSounds : MechComponent
{
	AudioSource singleSource;
	[SerializeField] AudioSource loopSource;
	[SerializeField] AudioClip moveArmBeginSound;
	[SerializeField] AudioClip moveArmSound;
	[SerializeField] AudioClip moveArmEndSound;
	[SerializeField] AudioClip bodyHitSound;

	protected override void OnAwake()
	{
		base.OnAwake();
		singleSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		//arms.armBlockState.OnMoveArmBegin -= PlayOnMoveBeginSound;
		//arms.armBlockState.OnMoveArmBegin += PlayOnMoveBeginSound;
		//arms.armBlockState.OnMoveArm -= PlayOnMoveSound;
		//arms.armBlockState.OnMoveArm += PlayOnMoveSound;
		//arms.armBlockState.OnMoveArmEnd -= PlayOnMoveEndSound;
		//arms.armBlockState.OnMoveArmEnd += PlayOnMoveEndSound;
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

	float RandomPitch(float basePitch, float maxVariance)
	{
		return basePitch + Random.Range(-maxVariance, maxVariance);
	}

	public void PlayBodyHitSound(float impact = 1f)
	{
		impact *= 20f;
		singleSource.volume = impact;
		//print(impact);
		singleSource.pitch = RandomPitch(1, 0.2f);
		singleSource.PlayOneShot(bodyHitSound);
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