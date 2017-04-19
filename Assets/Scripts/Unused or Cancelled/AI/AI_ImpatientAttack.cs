//using System.Collections;
using UnityEngine;

public class AI_ImpatientAttack : AI_AttackMethod
{
#if LEGACY

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void RunComponent()
	{
		base.RunComponent();

		print("In impatient attack");
	}

	protected override void Update()
	{
		base.Update();
	}
#endif
}