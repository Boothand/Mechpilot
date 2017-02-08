using UnityEngine;
using System.Collections;

//Responsible for all movement of the mech, including crouching, dodging and dashing, and syncing those animations.
public class Helm : MechComponent
{
	//Auto-find references
	public HeadRotation headRotation { get; private set; }
	public MechMovement move { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();
		headRotation = GetComponentInChildren<HeadRotation>();
		move = GetComponentInChildren<MechMovement>();
	}

	void FixedUpdate()
	{
		//move.RunComponentFixed();
	}

	void Update ()
	{
		//------------------ MOVING / TURNING THE BODY ------------------\\

		//move.RunComponent();

		//------------------ LOOKING AROUND / ROTATING HEAD ------------------\\
		headRotation.RunComponent();
	}
}