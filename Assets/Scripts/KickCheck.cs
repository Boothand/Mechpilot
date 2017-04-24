//using System.Collections;
using UnityEngine;

//Used mostly for type checking when its trigger volume is detected by something relevant.
//Note that it inherits from Collidable which sends collision/trigger events.
public class KickCheck : Collidable
{
	public Kicker kicker { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();
		kicker = mech.transform.root.GetComponentInChildren<Kicker>();
	}
}