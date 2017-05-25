//using System.Collections;
using UnityEngine;

//This component keeps track of all audio clips, and plays them on the appropriate audio source.
public class MechSounds : MechComponent
{
	AudioSource singleSource;	//For sounds that play once per action.
	[SerializeField] AudioSource loopSource;	//For sounds that loop or play continously.
	[SerializeField] AudioClip moveArmBeginSound;
	[SerializeField] AudioClip moveArmSound;
	[SerializeField] AudioClip moveArmEndSound;
	[SerializeField] AudioClip bodyHitSound;
	[SerializeField] AudioClip swordSwingSound;
	[SerializeField] AudioClip[] footstepSounds;

	protected override void OnAwake()
	{
		base.OnAwake();
		singleSource = GetComponent<AudioSource>();
	}

	protected override void OnStart()
	{
		base.OnStart();

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

	float RandomPitch(float basePitch, float maxVariance)
	{
		return basePitch + Random.Range(-maxVariance, maxVariance);
	}

	public void PlayBodyHitSound(float impact = 1f)
	{
		singleSource.volume = impact;
		singleSource.pitch = RandomPitch(1f, 0.1f);
		singleSource.PlayOneShot(bodyHitSound, 1.5f);
	}

	public void PlaySwordSwingSound()
	{
		singleSource.pitch = RandomPitch(1f, 0.2f);
		singleSource.volume = 1f;
		singleSource.PlayOneShot(swordSwingSound, 0.5f);
	}

	public void PlayFootStepSound()
	{
		singleSource.pitch = RandomPitch(1f, 0.02f);
		float volume = 0.4f * mech.pilot.move.getVelocity.magnitude;
		singleSource.volume = Mathf.Clamp(volume, 0.15f, 1f);

		AudioClip randomClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
		singleSource.PlayOneShot(randomClip);
	}
}