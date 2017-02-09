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

	[SerializeField] BodyGroupStats[] bodyGroupStats;



	void Awake()
	{

	}

	public void GetHit(BodyPart.BodyGroup group)
	{
		int index = (int)group;
		bodyGroupStats[index].hitCount++;
		print(bodyGroupStats[index].name + " got hit by a sword for the " + bodyGroupStats[index].hitCount + ". time.");
		health -= bodyGroupStats[index].damage;

		health = Mathf.Clamp(health, 0, maxHealth);
	}

	void Update()
	{
		
	}
}