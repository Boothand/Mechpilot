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
	protected Helm helm;
	protected Engineer engineer;
	protected WeaponsOfficer arms;
	protected EnergyManager energyManager;

	protected const int HELM_INDEX = 0;
	protected const int ARMS_INDEX = 1;

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
		helm = mech.transform.root.GetComponentInChildren<Helm>();
		engineer = mech.transform.root.GetComponentInChildren<Engineer>();
		arms = mech.transform.root.GetComponentInChildren<WeaponsOfficer>();
		energyManager = mech.transform.root.GetComponentInChildren<EnergyManager>();

		scaleFactor = transform.root.localScale.y;
		base.OnAwake();
	}
}