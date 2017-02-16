//using System.Collections;
using UnityEngine;

public class KickCheck : Collidable
{
	public Kicker kicker { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		kicker = mech.transform.root.GetComponentInChildren<Kicker>();
	}

	void Update()
	{
		
	}
}