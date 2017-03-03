using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	public ArmControl armControl { get; private set; }
	public ArmBlockState armBlockState { get; private set; }
	public ArmWindupState armWindupState { get; private set; }
	public ArmAttackState armAttackState { get; private set; }
	public ArmStaggerState armStaggerState { get; private set; }

	public enum CombatState { Stance, Block, Attack, Retract }
	public CombatState combatState;
	public enum CombatDir { TopRight, TopLeft, BottomRight, BottomLeft, Top }

	public Vector3 inputVec { get; private set; }
	public float inputVecMagnitude { get; private set; }

	[SerializeField] Sword weapon;
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] public Transform lHandIKTarget;

	public Sword getWeapon { get { return weapon; } }
	public Transform getRhandIKTarget { get { return rHandIKTarget; } }

	protected override void OnAwake()
	{
		base.OnAwake();
		
		armControl = GetComponent<ArmControl>();
		armBlockState = GetComponent<ArmBlockState>();
		armWindupState = GetComponent<ArmWindupState>();
		armAttackState = GetComponent<ArmAttackState>();
		armStaggerState = GetComponent<ArmStaggerState>();
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
		//IgnoreHierarchyRecursive(transform.root, weapon.GetComponent<Collider>());
	}

	public WeaponsOfficer.CombatDir DecideCombatDir(WeaponsOfficer.CombatDir inDir)
	{
		if (Mathf.Abs(arms.inputVec.x) < 0.2f &&
			arms.inputVec.y > 0.2f)
		{
			return WeaponsOfficer.CombatDir.Top;
		}

		if (arms.inputVec.x > 0.1f)
		{
			if (arms.inputVec.y < -0f)
			{
				//Top right
				return WeaponsOfficer.CombatDir.BottomRight;
			}

			//Bottom right
			return WeaponsOfficer.CombatDir.TopRight;
		}

		if (arms.inputVec.x < -0.1f)
		{
			if (arms.inputVec.y < -0f)
			{
				//Bottom left
				return WeaponsOfficer.CombatDir.BottomLeft;
			}

			//Top left
			return WeaponsOfficer.CombatDir.TopLeft;
		}

		//Default
		return inDir;
	}

	void FixedUpdate()
	{
		//armMovement.RunComponent();

	}

	void Update ()
	{
		inputVec = new Vector3(input.rArmHorz, input.rArmVert).normalized;
		inputVecMagnitude = inputVec.magnitude;

		lHandIKTarget.position = weapon.getLeftHandTarget.position;
		lHandIKTarget.rotation = weapon.getLeftHandTarget.rotation;
	}
}