using System.Collections;
using UnityEngine;

[System.Serializable]
public class BodyGroupStats
{
	public string name = "Unnamed BodyGroup";
	public int damage = 20;
	public int hitCount { get; set; }
}

public class HealthManager : MechComponent
{
	int health;
	[SerializeField] int startHealth = 100;
	[SerializeField] int maxHealth = 100;
	public int getHealth { get { return health; } }
	public int getMaxHealth { get { return maxHealth; } }
	public bool takingDamage { get; set; }
	public bool dead { get; set; }

	[SerializeField] BodyGroupStats[] bodyGroupStats;
	
	public System.Action<Vector3> OnGetHit;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		health = startHealth;
	}

	public void GetHit(BodyPart.BodyGroup group, Vector3 velocity, Vector3 hitPoint, int overrideDamage = -1)
	{
		int index = (int)group;
		float velocityMagnitude = velocity.magnitude;	//Calculate once since we use it many times.

		//Impact should be roughly in the range 0.5 to 1.5 (like a multiplier),
		//decided by the tip of the sword's velocity.
		float impact = velocityMagnitude * 30f;
		//print("Impact: " + impact);
		impact = Mathf.Clamp(impact, 0.5f, 1.5f);

		//Subtract the relevant damage from health, modified by the impact.
		int finalDamage = (int)(bodyGroupStats[index].damage * impact);

		//Allow overriding the damage if we need it to be something specific.
		if (overrideDamage > 0)
		{
			finalDamage = overrideDamage;
		}

		health -= finalDamage;


		//If we need to detect how many hits a bodypart has.
		bodyGroupStats[index].hitCount++;
		//print(bodyGroupStats[index].name + " got hit by a sword for the " + bodyGroupStats[index].hitCount + ". time.");

		//Play a hit sound, modified by the velocity of the other's sword
		mechSounds.PlayBodyHitSound(0.8f * impact);

		//Make upper body more floppy when hit, so it's visible that it impacted us.
		//if (!dead)
		//{
			//arms.SetPinWeightUpperBody(1f, 0.3f, 0f);
			//arms.puppet.muscles[arms.puppet.GetMuscleIndex(HumanBodyBones.Spine)].rigidbody.AddForceAtPosition(velocity * 5000f, hitPoint, ForceMode.Impulse);

			//arms.SetPinWeightUpperBody(0.3f, 1f, 1f);

		//}

		//If we die from the damage
		if (health <= 0f)
		{
			Die();
		}

		health = Mathf.Clamp(health, 0, maxHealth);

		if (OnGetHit != null)
			OnGetHit(hitPoint);
	}

	//Go into ragdoll
	IEnumerator DieRoutine()
	{
		//Tween pin weight to 0 before KillPuppet, so it doesn't look buggy.
		arms.SetPinWeightWholeBody(1f, 0f, 0.3f);

		yield return new WaitForSecondsRealtime(0.5f);

		arms.KillPuppet();
	}

	public void Die()
	{
		dead = true;
		StartCoroutine(DieRoutine());
	}
}