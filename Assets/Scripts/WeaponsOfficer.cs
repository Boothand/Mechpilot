using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	public ArmMovement armMovement { get; private set; }
	public ArmRotation armRotation { get; private set; }
	[SerializeField] Transform weapon;

	protected override void OnAwake()
	{
		base.OnAwake();

		armMovement = GetComponent<ArmMovement>();
		armRotation = GetComponent<ArmRotation>();
	}

	void IgnoreHierarchyRecursive(Transform root, Collider otherCol)
	{
		foreach (Transform child in root)
		{
			Collider col = child.GetComponent<Collider>();

			if (col)
			{
				Physics.IgnoreCollision(col, otherCol);
			}

			IgnoreHierarchyRecursive(child, otherCol);
		}
	}

	void Start()
	{
		IgnoreHierarchyRecursive(transform.root, weapon.GetComponent<Collider>());
	}

	void FixedUpdate()
	{
		//armMovement.RunComponent();

	}

	void Update ()
	{
		//Move IK targets horizontally and vertically
		armMovement.RunComponent();

		//Run attack animations, manage attacking states and gameplay
	}
}