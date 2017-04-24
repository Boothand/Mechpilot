//using System.Collections;
using UnityEngine;

//Increases and decreases the 'stamina' or energy.
public class EnergyManager : MechComponent
{
	public float stamina { get; private set; }
	[SerializeField] float maxStamina = 100f;
	[SerializeField] float regenerationSpeed = 5f;

	public float getMaxStamina { get { return maxStamina; } }

	//Let others know when we start spending stamina.
	public System.Action<Vector3> OnSpendStamina;


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		stamina = maxStamina;
	}

	public void SpendStamina(float amount)
	{
		stamina -= amount;

		if (OnSpendStamina != null)
			OnSpendStamina(Vector3.zero);
	}

	public bool CanSpendStamina(float amount)
	{
		return stamina - amount > 0f;
	}

	void Update()
	{
		//Regenerate
		if (stamina < maxStamina)
		{
			if (arms.combatState != WeaponsOfficer.CombatState.Attack)
			{
				stamina += Time.deltaTime * regenerationSpeed;
			}
		}

		//Don't go below 0 or above max.
		stamina = Mathf.Clamp(stamina, 0f, maxStamina);
	}
}