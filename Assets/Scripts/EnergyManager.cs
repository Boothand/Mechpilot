//using System.Collections;
using UnityEngine;

public class EnergyManager : MechComponent
{
	public float stamina { get; private set; }
	[SerializeField] float maxStamina = 100f;
	[SerializeField] float regenerationSpeed = 5f;

	public float getMaxStamina { get { return maxStamina; } }

	public System.Action OnSpendStamina;

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
			OnSpendStamina();
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

		stamina = Mathf.Clamp(stamina, 0f, maxStamina);
	}
}