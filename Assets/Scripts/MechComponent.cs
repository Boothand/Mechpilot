using UnityEngine;
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
	public WeaponsOfficer arms { get; protected set; }
	protected EnergyManager energyManager;
	protected Dasher dasher;
	protected HealthManager healthManager;

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
		healthManager = mech.transform.root.GetComponentInChildren<HealthManager>();

		scaleFactor = transform.root.localScale.y;
		base.OnAwake();
	}
}