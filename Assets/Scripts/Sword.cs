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
	[SerializeField] LayerMask nonAttackIgnoreLayers;
	int ignoreLayerNum;

	protected override void OnAwake()
	{
		base.OnAwake();
		swordCollider = GetComponent<Collider>();
	}

	void Start()
	{
		ignoreLayerNum = GetLayerFromLayerMask(nonAttackIgnoreLayers);
	}

	public void PlayClashSound(float impact = 1f)
	{
		AudioClip randomClash = clashes[Random.Range(0, clashes.Length)];

		StartCoroutine(PlaySoundRoutine(randomClash, impact));
	}

	int GetLayerFromLayerMask(LayerMask mask)
	{
		int layerNum = 0;

		int layerValue = mask.value;

		while (layerValue > 0)
		{
			layerValue = layerValue >> 1;
			layerNum++;
		}

		layerNum--;
		
		return layerNum;
	}

	public void EnableCollider(bool truth)
	{
		swordCollider.enabled = truth;
	}

	public void SetCollisionWithIrrelevant(bool truth)
	{
		Physics.IgnoreLayerCollision(swordCollider.gameObject.layer, ignoreLayerNum, !truth);
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
		
		//Play clash sound
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

	void CalculateSwordTipVelocity()
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
	}

	void FixedUpdate()
	{
		//Calculate sword tip velocity
		CalculateSwordTipVelocity();
	}
}