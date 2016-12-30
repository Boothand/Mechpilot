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

	protected override void OnAwake()
	{
		animator = mech.GetComponent<Animator>();
		hierarchy = mech.GetComponent<BodyHierarchy>();
		rb = mech.GetComponent<Rigidbody>();
		input = mech.GetComponent<AbstractInput>();
		helm = transform.root.GetComponentInChildren<Helm>();
		engineer = transform.root.GetComponentInChildren<Engineer>();
		arms = transform.root.GetComponentInChildren<WeaponsOfficer>();

		base.OnAwake();
	}
}