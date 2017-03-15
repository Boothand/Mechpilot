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
	int health = 100;
	[SerializeField] int maxHealth = 100;
	public int getHealth { get { return health; } }
	public int getMaxHealth { get { return maxHealth; } }
	public bool takingDamage { get; set; }
	public bool dead { get; set; }

	[SerializeField] BodyGroupStats[] bodyGroupStats;

	public delegate void Hit();
	public event Hit OnGetHit;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		health = maxHealth;
	}

	public void GetHit(BodyPart.BodyGroup group, Vector3 velocity)
	{
		float impact = velocity.magnitude * 10f;
		//print("Impact: " + impact);
		impact = Mathf.Clamp(impact, 0.5f, 1.5f);
		//print(impact);
		int index = (int)group;
		bodyGroupStats[index].hitCount++;
		//print(bodyGroupStats[index].name + " got hit by a sword for the " + bodyGroupStats[index].hitCount + ". time.");
		int finalDamage = (int)(bodyGroupStats[index].damage * impact);
		health -= finalDamage;
		mechSounds.PlayBodyHitSound(1f);

		if (dead)
		{
			arms.SetPinWeightUpperBody(1f, 0.4f, 0f);
			arms.SetPinWeightUpperBody(0.4f, 2f, 1f);
		}

		if (health <= 0f)
		{
			Die();
			dead = true;
		}

		health = Mathf.Clamp(health, 0, maxHealth);

		if (OnGetHit != null)
		{
			OnGetHit();
		}
	}

	IEnumerator DieRoutine()
	{
		arms.SetPinWeightWholeBody(1f, 0f, 0.3f);

		yield return new WaitForSeconds(0.5f);

		arms.KillPuppet();
	}

	public void Die()
	{
		StartCoroutine(DieRoutine());
		//arms.KillPuppet();
	}

	void Update()
	{
		
	}
}