//using System.Collections;
using UnityEngine;

public class AI_CounterAttack : AI_AttackMethod
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		print("In counter attack");
	}

	protected override void Update()
	{
		base.Update();
	}
}