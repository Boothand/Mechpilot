//using System.Collections;
using UnityEngine;

[System.Serializable]
public class BodyGroupStats
{
	public string name = "Unnamed BodyGroup";
	public int damage = 20;
	public int hitCount { get; set; }
}

public class HealthManager : MonoBehaviour
{
	int health = 100;
	[SerializeField] int maxHealth = 100;
	public int getHealth { get { return health; } }
	public int getMaxHealth { get { return maxHealth; } }
	public bool takingDamage { get; set; }

	[SerializeField] BodyGroupStats[] bodyGroupStats;

	public delegate void Hit();
	public event Hit OnGetHit;


	void Awake()
	{

	}

	public void GetHit(BodyPart.BodyGroup group, Vector3 velocity)
	{
		float impact = velocity.magnitude;

		//print(impact);
		int index = (int)group;
		bodyGroupStats[index].hitCount++;
		//print(bodyGroupStats[index].name + " got hit by a sword for the " + bodyGroupStats[index].hitCount + ". time.");
		int finalDamage = (int)(bodyGroupStats[index].damage * impact);
		health -= finalDamage;

		health = Mathf.Clamp(health, 0, maxHealth);

		if (OnGetHit != null)
		{
			OnGetHit();
		}

		//print(health);
	}

	void Update()
	{
		
	}
}