using UnityEngine;
//using System.Collections;

//Base class for fully aware mech components. It provides a path to any other component on the mech.
public class MechComponent : ManagedMechBehaviour
{
	//Components common for both pilot and weapons officer:
	protected Animator animator;
	protected BodyHierarchy hierarchy;
	protected Rigidbody rb;
	protected AbstractInput input;
	protected EnergyManager energyManager;
	public HealthManager healthManager { get; private set; }
	protected MechSounds mechSounds;
	public CameraFollow cameraFollow { get; private set; }
	public GroundCheck groundCheck { get; private set; }
	public SwordClashParticles swordClashParticles { get; private set; }

	//The two roles:
	protected Pilot pilot { get; private set; }
	public WeaponsOfficer arms { get; private set; }
	//protected Engineer engineer;	

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
		energyManager = mech.transform.root.GetComponentInChildren<EnergyManager>();
		healthManager = mech.transform.root.GetComponentInChildren<HealthManager>();
		mechSounds = mech.transform.root.GetComponentInChildren<MechSounds>();
		cameraFollow = transform.root.GetComponentInChildren<CameraFollow>();
		swordClashParticles = transform.root.GetComponentInChildren<SwordClashParticles>();
		groundCheck = transform.root.GetComponentInChildren<GroundCheck>();

		pilot = mech.transform.root.GetComponentInChildren<Pilot>();
		arms = mech.transform.root.GetComponentInChildren<WeaponsOfficer>();
		//engineer = mech.transform.root.GetComponentInChildren<Engineer>();
		
		scaleFactor = transform.root.localScale.y;
		
		base.OnAwake();
	}
}