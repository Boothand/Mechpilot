using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	public ArmControl armControl { get; private set; }
	public ArmBlockState armBlockState { get; private set; }
	public ArmWindupState armWindupState { get; private set; }
	public ArmAttackState armAttackState { get; private set; }
	[SerializeField] Sword weapon;

	public Sword getWeapon { get { return weapon; } }

	protected override void OnAwake()
	{
		base.OnAwake();
		
		armControl = GetComponent<ArmControl>();
		armBlockState = GetComponent<ArmBlockState>();
		armWindupState = GetComponent<ArmWindupState>();
		armAttackState = GetComponent<ArmAttackState>();
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
		IgnoreHierarchyRecursive(transform.root, weapon.transform.GetChild(0).GetComponent<Collider>());
	}

	void FixedUpdate()
	{
		//armMovement.RunComponent();

	}

	void Update ()
	{
		//Move IK targets horizontally and vertically
		//armMovement.RunComponent();

		//Run attack animations, manage attacking states and gameplay
	}
}