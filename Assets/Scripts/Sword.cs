using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Collidable
{
	//Velocity
	[SerializeField] Transform swordTip;
	public Transform getSwordTip { get { return swordTip; } }
	public Vector3 swordTipVelocity { get; private set; }	//Figure out how fast the sword has moved/rotated.
	List<Vector3> velocityList = new List<Vector3>();
	Vector3 averagePosition;

	//Sound
	[SerializeField] AudioSource audioSource;	//Play sword sounds on the sword, not on the mech
	[SerializeField] AudioClip[] clashes;	//All the clash sounds.
	public bool playingSwordSound { get; private set; }
	float timeSinceLastClash;

	//Collision and physics
	Collider swordCollider;
	ConfigurableJoint configJoint;
	bool anglesLocked;

	public System.Action<Vector3, Sword> OnClashWithSword;


	protected override void OnAwake()
	{
		base.OnAwake();
		swordCollider = GetComponent<Collider>();
		configJoint = GetComponent<ConfigurableJoint>();
	}

	void Start()
	{
		//Hack to set the layer one frame after start, overriding the layer set by PuppetMaster
		StartCoroutine(CorrectWeaponLayerRoutine());
	}

	IEnumerator CorrectWeaponLayerRoutine()
	{
		yield return null;
		gameObject.layer = 10;
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

	//Whether the sword can collide with the enemy and the ground
	public void SetCollisionWithBodyAndDefault(bool truth)
	{
		if (truth)
		{
			gameObject.layer = 10;
		}
		else
		{
			gameObject.layer = 13;
		}
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

	//When the sword collides with something:
	protected override void RunCollisionEvent(Collision col)
	{
		base.RunCollisionEvent(col);
		Sword otherSword = col.transform.GetComponent<Sword>();
		
		//If we hit another sword, play clash sound
		if (otherSword)
		{
			if ( (arms.prevCombatState == WeaponsOfficer.CombatState.Attack
			|| arms.prevCombatState == WeaponsOfficer.CombatState.Block)
			&& (otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Attack
				|| otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Block))
			{
				float magnitude = col.relativeVelocity.magnitude;

				//Don't spam the sound!
				if (timeSinceLastClash > 0.5f &&
					!otherSword.playingSwordSound)
				{
					timeSinceLastClash = 0f;

					if (OnClashWithSword != null)
						OnClashWithSword(col.contacts[0].point, otherSword);

					PlayClashSound(magnitude * 0.15f);
				}
			}
		}

		//If we collide with something, don't force the sword to be locked to the hand's motion.
		LockSwordAngularMotion(false);
		anglesLocked = false;
	}

	//Check how fast the tip of the sword has moved over several frames.
	void CalculateSwordTipVelocity()
	{
		//Velocity = current position - last position (where last position is an average).
		swordTipVelocity = (swordTip.position - averagePosition) * Time.deltaTime;
		swordTipVelocity *= scaleFactor;	//Scale factor was only done to support different sized mechs..

		//Make sure there are 5 entries in the list, where the last element is the most recent.
		if (velocityList.Count < 5)
		{
			velocityList.Add(swordTip.position);
		}
		else
		{
			velocityList.RemoveAt(0);
		}

		//Find the average position of all entries.
		averagePosition = Vector3.zero;

		for (int i = 0; i < velocityList.Count; i++)
		{
			averagePosition += velocityList[i];
		}

		averagePosition /= velocityList.Count;
	}

	//Whether to fix the sword's rotation to the hand rotation or not.
	public void LockSwordAngularMotion(bool truth)
	{
		if (truth)
		{
			configJoint.angularXMotion = ConfigurableJointMotion.Locked;
			configJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configJoint.angularZMotion = ConfigurableJointMotion.Locked;
		}
		else
		{
			configJoint.angularXMotion = ConfigurableJointMotion.Free;
			configJoint.angularYMotion = ConfigurableJointMotion.Free;
			configJoint.angularZMotion = ConfigurableJointMotion.Free;
		}
	}

	protected override void OnFixedUpdate()
	{
		//Calculate sword tip velocity
		CalculateSwordTipVelocity();
	}

	protected override void OnUpdate()
	{
		//Turn off collider when not blocking or attacking
		if (arms.combatState == WeaponsOfficer.CombatState.Attack
			|| arms.combatState == WeaponsOfficer.CombatState.Block
			|| arms.combatState == WeaponsOfficer.CombatState.Stagger
			|| arms.stancePicker.changingStance
			|| healthManager.dead)
		{
			EnableCollider(true);
		}
		else
		{
			EnableCollider(false);
		}

		//If we attack, retract or stagger, collide with their body and default colliders.
		//Only doing this selectively so the sword doesn't get stuck underneath someone or
		//somehow not returning where it should be in time.
		if (arms.combatState == WeaponsOfficer.CombatState.Attack
			|| arms.combatState == WeaponsOfficer.CombatState.Retract
			|| arms.combatState == WeaponsOfficer.CombatState.Stagger)
		{
			SetCollisionWithBodyAndDefault(true);
		}
		else
		{
			SetCollisionWithBodyAndDefault(false);
		}

		//Lock the angles again once we're in stance.
		if (!anglesLocked
			&& arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			LockSwordAngularMotion(true);
			anglesLocked = true;
		}

		timeSinceLastClash += Time.deltaTime;
	}
}