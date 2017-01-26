//using System.Collections;
using UnityEngine;

public class AI_MechComponent : MechComponent
{
	protected Mech enemy;

	protected override void OnAwake()
	{
		base.OnAwake();

		Mech[] mechs = FindObjectsOfType<Mech>();

		for (int i = 0; i < mechs.Length; i++)
		{
			if (mechs[i] != mech)
			{
				enemy = mechs[i];
				break;
			}
		}
	}

	void Update()
	{

	}
}