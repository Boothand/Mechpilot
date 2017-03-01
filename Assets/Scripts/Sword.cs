using System.Collections;
using UnityEngine;

public class Sword : Collidable
{
	[SerializeField] Transform swordTip;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip[] clashes;
	
	public bool playingSwordSound { get; private set; }
	public Transform getSwordTip { get { return swordTip; } }
	public Vector3 swordTipVelocity { get; private set; }

	Vector3 lastPos;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public void PlayClashSound(float impact = 1f)
	{
		AudioClip randomClash = clashes[Random.Range(0, clashes.Length)];

		StartCoroutine(PlaySoundRoutine(randomClash, impact));
	}

	IEnumerator PlaySoundRoutine(AudioClip clip, float volume)
	{
		playingSwordSound = true;

		audioSource.volume = volume;
		//float pitch = 1 + Random.Range(-0.1f, 0.1f);
		audioSource.PlayOneShot(clip);

		yield return new WaitForSeconds(0.2f);

		playingSwordSound = false;
	}

	protected override void RunCollisionEvent(Collision col)
	{
		base.RunCollisionEvent(col);

		if (col.transform.GetComponent<Sword>())
		{
			float magnitude = col.relativeVelocity.magnitude;
			PlayClashSound(magnitude * 0.15f);
		}
	}

	void Update()
	{
		swordTipVelocity = swordTip.position - lastPos;
		swordTipVelocity *= scaleFactor;

		lastPos = swordTip.position;
	}
}