using System.Collections;
using UnityEngine;

public class AI_AttackMethod : AI_MechComponent
{


	protected override void OnAwake()
	{
		base.OnAwake();
	}


	public virtual void RunComponent()
	{
		aiCombat.ZeroAllInputs();
	}

	protected override void Update()
	{
		base.Update();
	}
}