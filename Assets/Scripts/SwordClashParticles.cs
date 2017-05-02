using System.Collections;
using UnityEngine;

//Plays a particle system at the clash location.
public class SwordClashParticles : MechComponent
{
	[SerializeField] ParticleSystem pSystem;
	public bool playingParticles { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void OnStart()
	{
		base.OnStart();

		//Callback when swords clash
		arms.getWeapon.OnClashWithSword += PlayParticleEffect;
	}

	void PlayParticleEffect(Vector3 location, Sword otherSword)
	{
		if (pSystem
			&& !otherSword.swordClashParticles.playingParticles)
		{
			playingParticles = true;
			pSystem.transform.position = location;
			pSystem.Emit(50);   //Don't use Play(), places it at the wrong location.

			//Just so it's detectable for others whether you're playing particles or not:
			StartCoroutine(PlayParticleDelayed());	
		}
	}

	IEnumerator PlayParticleDelayed()
	{
		yield return null;

		playingParticles = false;
	}
}