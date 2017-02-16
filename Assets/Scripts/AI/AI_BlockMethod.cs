//using System.Collections;
using UnityEngine;

public class AI_BlockMethod : AI_MechComponent
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