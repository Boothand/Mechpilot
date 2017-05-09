using UnityEngine;
using System.Collections;

//Responsible for all movement of the mech, including crouching, dodging and dashing, and syncing those animations.
public class Pilot : MechComponent
{
	//Auto-find references
	public MechRotation headRotation { get; private set; }
	public MechMovement move { get; private set; }
	public Dasher dasher { get; private set; }
	public Croucher croucher { get; private set; }
	public Dodge dodger { get; private set; }
	public FootStanceSwitcher footStanceSwitcher { get; private set; }
	public Run run { get; private set; }
	public LockonIndicator lockonIndicator { get; private set; }
	public Lockon lockOn { get; private set; }
	public Kicker kicker { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();
		headRotation = mech.transform.root.GetComponentInChildren<MechRotation>();
		move = mech.transform.root.GetComponentInChildren<MechMovement>();
		dasher = mech.transform.root.GetComponentInChildren<Dasher>();
		croucher = mech.transform.root.GetComponentInChildren<Croucher>();
		dodger = mech.transform.root.GetComponentInChildren<Dodge>();
		footStanceSwitcher = transform.root.GetComponentInChildren<FootStanceSwitcher>();
		run = transform.root.GetComponentInChildren<Run>();
		lockonIndicator = transform.root.GetComponentInChildren<LockonIndicator>();
		lockOn = transform.root.GetComponentInChildren<Lockon>();
		kicker = transform.root.GetComponentInChildren<Kicker>();
	}

	protected override void OnFixedUpdate()
	{
		//move.RunComponentFixed();
	}

	protected override void OnUpdate ()
	{
		if (input.restartScene)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		
		//if (healthManager.dead)
		//{
		//	input.enabled = false;
		//}
	}
}