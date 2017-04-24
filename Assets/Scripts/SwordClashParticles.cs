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

	void Start()
	{
		//Callback when swords clash
		arms.getWeapon.OnClashWithSword += PlayParticleEffect;
	}

	void PlayParticleEffect(Vector3 location, Sword otherSword)
	{
		if (pSystem
			&& !otherSword.swordClashParticles.playingParticles)
		{
			playingParticles = true;
			Debug.DrawLine(arms.getWeapon.transform.position, location, Color.red);
			//UnityEditor.EditorApplication.isPaused = true;
			pSystem.transform.position = location;
			pSystem.Emit(50);

			StartCoroutine(PlayParticleDelayed());
		}
	}

	IEnumerator PlayParticleDelayed()
	{
		yield return null;

		playingParticles = false;
	}

	void Update()
	{
		
	}
}