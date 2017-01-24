using System.Collections;
using UnityEngine;

public class AI_Attacker : MechComponent
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		StartCoroutine(AttackRoutine());
	}

	IEnumerator AttackRoutine()
	{
		while (true)
		{
			input.attack = true;

			while (arms.weaponControl.state != ArmRotation.State.WindUp)
			{
				yield return null;
			}

			yield return new WaitForSeconds(0.1f);

			input.attack = false;

			while (arms.weaponControl.state != ArmRotation.State.Defend)
			{
				yield return null;
			}
		}
	}

	void Update()
	{
		
	}
}