using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Collidable
{
	[SerializeField] Transform swordTip;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip[] clashes;
	
	public bool playingSwordSound { get; private set; }
	public Transform getSwordTip { get { return swordTip; } }
	public Vector3 swordTipVelocity { get; private set; }
	[SerializeField] Transform leftHandTarget;
	[SerializeField] Transform midPoint;
	public Transform getLeftHandTarget { get { return leftHandTarget; } }
	public Transform getMidPoint { get { return midPoint; } }
	Collider swordCollider;
	List<Vector3> velocityList = new List<Vector3>();
	Vector3 averagePosition;

	protected override void OnAwake()
	{
		base.OnAwake();
		swordCollider = GetComponent<Collider>();
	}

	public void PlayClashSound(float impact = 1f)
	{
		AudioClip randomClash = clashes[Random.Range(0, clashes.Length)];

		StartCoroutine(PlaySoundRoutine(randomClash, impact));
	}

	public void EnableCollider(bool truth)
	{
		swordCollider.enabled = truth;
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
		Sword otherSword = col.transform.GetComponent<Sword>();
		
		if (otherSword)
		{
			if (arms.prevCombatState == WeaponsOfficer.CombatState.Attack
			|| arms.prevCombatState == WeaponsOfficer.CombatState.Block
			&& (otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Attack
				|| otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Block))
			{
				float magnitude = col.relativeVelocity.magnitude;
				PlayClashSound(magnitude * 0.15f);
			}
		}
	}

	void FixedUpdate()
	{
		swordTipVelocity = (swordTip.position - averagePosition) * Time.deltaTime;
		swordTipVelocity *= scaleFactor;

		if (velocityList.Count < 5)
		{
			velocityList.Add(swordTip.position);
		}
		else
		{
			velocityList.RemoveAt(0);
		}

		averagePosition = Vector3.zero;

		for (int i = 0; i < velocityList.Count; i++)
		{
			averagePosition += velocityList[i];
		}

		averagePosition /= velocityList.Count;

		//Debug.DrawRay(swordTip.position, swordTipVelocity, Color.red);
	}
}