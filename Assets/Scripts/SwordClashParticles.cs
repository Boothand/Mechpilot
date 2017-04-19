using System.Collections;
using UnityEngine;

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
			pSystem.Play();


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