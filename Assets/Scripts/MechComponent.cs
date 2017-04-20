﻿using UnityEngine;
//using System.Collections;

//Base class for fully aware mech components 
public class MechComponent : ManagedMechBehaviour
{
	//Auto-get
	protected Animator animator;
	protected BodyHierarchy hierarchy;
	protected Rigidbody rb;
	protected AbstractInput input;
	protected Pilot pilot;
	protected Engineer engineer;
	public WeaponsOfficer arms { get; private set; }
	public EnergyManager energyManager { get; private set; }
	protected Dasher dasher;
	protected Croucher croucher;
	public HealthManager healthManager { get; private set; }
	protected MechSounds mechSounds;
	public Blocker blocker { get; private set; }
	public Attacker attacker { get; private set; }
	public StancePicker stancePicker { get; private set; }
	public Windup windup { get; private set; }
	public Retract retract { get; private set; }
	public Dodge dodger { get; private set; }
	public Stagger stagger { get; private set; }
	public CameraFollow cameraFollow { get; private set; }
	public SwordClashParticles swordClashParticles { get; private set; }
	public FootStanceSwitcher footStanceSwitcher { get; private set; }
	public Lockon lockOn { get; private set; }
	public GroundCheck groundCheck { get; private set; }
	public Run run { get; private set; }

	protected const int PILOT_INDEX = 0;
	protected const int WEAPONOFFICER_INDEX = 1;

	protected float scaleFactor { get; private set; }

	protected override void OnAwake()
	{
		if (!mech)
		{
			Debug.LogWarning("No mech assigned on " + transform.name, this as Object);
			return;
		}
		animator = mech.GetComponent<Animator>();
		hierarchy = mech.GetComponent<BodyHierarchy>();
		rb = mech.GetComponent<Rigidbody>();
		input = mech.GetComponent<AbstractInput>();
		pilot = mech.transform.root.GetComponentInChildren<Pilot>();
		engineer = mech.transform.root.GetComponentInChildren<Engineer>();
		arms = mech.transform.root.GetComponentInChildren<WeaponsOfficer>();
		energyManager = mech.transform.root.GetComponentInChildren<EnergyManager>();
		dasher = mech.transform.root.GetComponentInChildren<Dasher>();
		croucher = mech.transform.root.GetComponentInChildren<Croucher>();
		healthManager = mech.transform.root.GetComponentInChildren<HealthManager>();
		mechSounds = mech.transform.root.GetComponentInChildren<MechSounds>();
		blocker = mech.transform.root.GetComponentInChildren<Blocker>();
		attacker = mech.transform.root.GetComponentInChildren<Attacker>();
		stancePicker = mech.transform.root.GetComponentInChildren<StancePicker>();
		windup = mech.transform.root.GetComponentInChildren<Windup>();
		retract = mech.transform.root.GetComponentInChildren<Retract>();
		dodger = mech.transform.root.GetComponentInChildren<Dodge>();
		stagger = mech.transform.root.GetComponentInChildren<Stagger>();
		scaleFactor = transform.root.localScale.y;
		cameraFollow = transform.root.GetComponentInChildren<CameraFollow>();
		swordClashParticles = transform.root.GetComponentInChildren<SwordClashParticles>();
		footStanceSwitcher = transform.root.GetComponentInChildren<FootStanceSwitcher>();
		lockOn = transform.root.GetComponentInChildren<Lockon>();
		groundCheck = transform.root.GetComponentInChildren<GroundCheck>();
		run = transform.root.GetComponentInChildren<Run>();
		base.OnAwake();
	}
}