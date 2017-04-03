//using System.Collections;
using UnityEngine;

public class AI_BlockMethod : AI_MechComponent
{
#if LEGACY

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
#endif
}