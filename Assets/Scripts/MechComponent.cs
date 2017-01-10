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

	protected const int HELM_INDEX = 0;
	protected const int ARMS_INDEX = 1;
	protected const int ENGINEER_INDEX = 2;

	protected override void OnAwake()
	{
		animator = mech.GetComponent<Animator>();
		hierarchy = mech.GetComponent<BodyHierarchy>();
		rb = mech.GetComponent<Rigidbody>();
		input = mech.GetComponent<AbstractInput>();
		helm = mech.transform.root.GetComponentInChildren<Helm>();
		engineer = mech.transform.root.GetComponentInChildren<Engineer>();
		arms = mech.transform.root.GetComponentInChildren<WeaponsOfficer>();

		base.OnAwake();
	}
}