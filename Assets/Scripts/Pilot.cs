using UnityEngine;
using System.Collections;

//Responsible for all movement of the mech, including crouching, dodging and dashing, and syncing those animations.
public class Pilot : MechComponent
{
	//Auto-find references
	public MechRotation headRotation { get; private set; }
	public MechMovement move { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();
		headRotation = GetComponentInChildren<MechRotation>();
		move = GetComponentInChildren<MechMovement>();
	}

	void FixedUpdate()
	{
		//move.RunComponentFixed();
	}

	void Update ()
	{
		if (input.restartScene)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}