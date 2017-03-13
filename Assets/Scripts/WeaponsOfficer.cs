using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using RootMotion.Dynamics;

public class WeaponsOfficer : MechComponent
{
	public ArmControl armControl { get; private set; }
	public ArmBlockState armBlockState { get; private set; }
	public ArmWindupState armWindupState { get; private set; }
	public ArmAttackState armAttackState { get; private set; }
	public ArmStaggerState armStaggerState { get; private set; }

	public enum CombatState { Stance, Block, Windup, Attack, Stagger, Retract }
	public CombatState combatState;
	public CombatState prevCombatState;
	public enum CombatDir { TopRight, TopLeft, BottomRight, BottomLeft, Top }

	public Vector3 inputVec { get; private set; }
	public float inputVecMagnitude { get; private set; }

	FullBodyBipedIK fbbik;
	PuppetMaster puppet;

	[SerializeField] Sword weapon;
	[SerializeField] Transform rHandIKTarget;
	[SerializeField] Transform lHandIKTarget;
	[SerializeField] Transform rElbowTarget;
	[SerializeField] Transform lElbowTarget;
	[SerializeField] Transform rShoulderTarget;
	[SerializeField] Transform lShoulderTarget;
	[SerializeField] Transform bodyTarget;

	[SerializeField] bool alwaysBlock;
	[SerializeField] bool alwaysAttack;

	Vector3 fromRhandPos, fromRElbowPos, fromLElbowPos, fromRShoulderPos, fromLShoulderPos, fromBodyPos;
	Quaternion fromRhandRot;

	public Sword getWeapon { get { return weapon; } }
	public Transform getRhandIKTarget { get { return rHandIKTarget; } }
	public Transform getLhandIKTarget { get { return lHandIKTarget; } }
	public Transform getRElbowTarget { get { return rElbowTarget; } }
	public Transform getLElbowTarget { get { return lElbowTarget; } }
	public Transform getRShoulderTarget { get { return rShoulderTarget; } }
	public Transform getLShoulderTarget { get { return lShoulderTarget; } }
	public Transform getBodyTarget { get { return bodyTarget; } }

	protected override void OnAwake()
	{
		base.OnAwake();
		
		armControl = GetComponent<ArmControl>();
		armBlockState = GetComponent<ArmBlockState>();
		armWindupState = GetComponent<ArmWindupState>();
		armAttackState = GetComponent<ArmAttackState>();
		armStaggerState = GetComponent<ArmStaggerState>();
		fbbik = transform.root.GetComponentInChildren<FullBodyBipedIK>();
		puppet = transform.root.GetComponentInChildren<PuppetMaster>();
	}

	void Start()
	{
		//IgnoreHierarchyRecursive(transform.root, weapon.GetComponent<Collider>());
	}

	IEnumerator TweenIKWeightRoutine(float weight, float time)
	{
		float timer = 0f;
		float fromWeight = fbbik.solver.IKPositionWeight;

		while (timer < time)
		{
			timer += Time.deltaTime;

			fbbik.solver.IKPositionWeight = Mathf.Lerp(fromWeight, weight, timer / time);
			yield return null;
		}

		fbbik.solver.IKPositionWeight = weight;
	}

	public void TweenIKWeight(float weight, float time)
	{
		StartCoroutine(TweenIKWeightRoutine(weight, time));
	}

	public void StoreTargets()
	{
		//print("Targets were stored.");
		fromRhandPos = rHandIKTarget.position;
		fromRhandRot = rHandIKTarget.rotation;

		fromRElbowPos = rElbowTarget.position;
		fromLElbowPos = lElbowTarget.position;

		fromRShoulderPos = rShoulderTarget.position;
		fromLShoulderPos = lShoulderTarget.position;

		fromBodyPos = bodyTarget.position;
	}

	public void InterpolateIKPose(IKPose pose, float timer)
	{
		rHandIKTarget.position = Vector3.Lerp(fromRhandPos, pose.rHand.position, timer);
		rHandIKTarget.rotation = Quaternion.Lerp(fromRhandRot, pose.rHand.rotation, timer);

		rElbowTarget.position = Vector3.Lerp(fromRElbowPos, pose.rElbow.position, timer);
		lElbowTarget.position = Vector3.Lerp(fromLElbowPos, pose.lElbow.position, timer);

		rShoulderTarget.position = Vector3.Lerp(fromRShoulderPos, pose.rShoulder.position, timer);
		lShoulderTarget.position = Vector3.Lerp(fromLShoulderPos, pose.lShoulder.position, timer);

		bodyTarget.position = Vector3.Lerp(fromBodyPos, pose.body.position, timer);
	}

	public void InterpolateIKPose(IKPose pose, Vector3 handOffset, float timer)
	{
		rHandIKTarget.position = Vector3.Lerp(fromRhandPos, pose.rHand.position + handOffset, timer);
		rHandIKTarget.rotation = Quaternion.Lerp(fromRhandRot, pose.rHand.rotation, timer);

		rElbowTarget.position = Vector3.Lerp(fromRElbowPos, pose.rElbow.position, timer);
		lElbowTarget.position = Vector3.Lerp(fromLElbowPos, pose.lElbow.position, timer);

		rShoulderTarget.position = Vector3.Lerp(fromRShoulderPos, pose.rShoulder.position, timer);
		lShoulderTarget.position = Vector3.Lerp(fromLShoulderPos, pose.lShoulder.position, timer);

		bodyTarget.position = Vector3.Lerp(fromBodyPos, pose.body.position, timer);
	}

	public void OffsetIKTargets(Vector3 offset, float blendSpeed)
	{
		rHandIKTarget.position += offset;
		rElbowTarget.position += offset;
		lElbowTarget.position += offset;
		rShoulderTarget.position += offset;
		lShoulderTarget.position += offset;
		bodyTarget.position += offset;

		//rHandIKTarget.position = Vector3.Lerp(rHandIKTarget.position, rHandIKTarget.position + offset, Time.deltaTime * blendSpeed);
		////rHandIKTarget.rotation = Quaternion.Lerp(rHandIKTarget.rotation, pose.rHand.rotation, timer);

		//rElbowTarget.position = Vector3.Lerp(rElbowTarget.position, rElbowTarget.position + offset, Time.deltaTime * blendSpeed);
		//lElbowTarget.position = Vector3.Lerp(lElbowTarget.position, lElbowTarget.position + offset, Time.deltaTime * blendSpeed);

		//rShoulderTarget.position = Vector3.Lerp(rShoulderTarget.position, rShoulderTarget.position + offset, Time.deltaTime * blendSpeed);
		//lShoulderTarget.position = Vector3.Lerp(lShoulderTarget.position, lShoulderTarget.position + offset, Time.deltaTime * blendSpeed);

		//bodyTarget.position = Vector3.Lerp(bodyTarget.position, bodyTarget.position + offset, Time.deltaTime * blendSpeed);
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

	public WeaponsOfficer.CombatDir DecideCombatDir(WeaponsOfficer.CombatDir inDir)
	{
		if (Mathf.Abs(inputVec.x) < 0.4f &&
			inputVec.y > 0.4f)
		{
			return WeaponsOfficer.CombatDir.Top;
		}

		if (inputVec.x > 0.1f)
		{
			if (inputVec.y < -0f)
			{
				//Top right
				return WeaponsOfficer.CombatDir.BottomRight;
			}

			//Bottom right
			return WeaponsOfficer.CombatDir.TopRight;
		}

		if (inputVec.x < -0.1f)
		{
			if (inputVec.y < -0f)
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

	IEnumerator SetPinWeightUpperBodyRoutine(float fromWeight, float toWeight, float duration)
	{
		float startPinWeight = puppet.pinWeight;

		float timer = 0f;
		float interpolationWeight = 0f;

		//HACKY :D
		float rhandStartMass = puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightHand)].rigidbody.mass;
		float rarmStartMass = puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightLowerArm)].rigidbody.mass;
		float rupperArmStartMass = puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightUpperArm)].rigidbody.mass;
		float weaponStartMass = getWeapon.GetComponent<Rigidbody>().mass;
		float headStartMass = puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.Head)].rigidbody.mass;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightHand)].rigidbody.mass = 1f;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightLowerArm)].rigidbody.mass = 1f;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightUpperArm)].rigidbody.mass = 1f;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.Head)].rigidbody.mass = 200f;

		getWeapon.GetComponent<Rigidbody>().mass = 1f;

		while (timer < duration)
		{
			timer += Time.deltaTime;

			interpolationWeight = Mathf.Lerp(fromWeight, toWeight, timer / duration);
			puppet.SetMuscleWeightsRecursive(HumanBodyBones.Spine, 1f, interpolationWeight);

			yield return null;
		}

		//HACKY!
		puppet.SetMuscleWeightsRecursive(HumanBodyBones.Spine, 1f, toWeight);
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightHand)].rigidbody.mass = rhandStartMass;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightLowerArm)].rigidbody.mass = rarmStartMass;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.RightUpperArm)].rigidbody.mass = rupperArmStartMass;
		puppet.muscles[puppet.GetMuscleIndex(HumanBodyBones.Head)].rigidbody.mass = headStartMass;
		getWeapon.GetComponent<Rigidbody>().mass = weaponStartMass;
	}

	public void SetPinWeightUpperBody(float fromWeight, float toWeight, float time)
	{
		StartCoroutine(SetPinWeightUpperBodyRoutine(fromWeight, toWeight, time));
	}

	void Update ()
	{
		inputVec = new Vector3(input.rArmHorz, input.rArmVert).normalized;
		inputVecMagnitude = inputVec.magnitude;

		lHandIKTarget.position = weapon.getLeftHandTarget.position;
		lHandIKTarget.rotation = weapon.getLeftHandTarget.rotation;

		if (alwaysBlock)
			input.block = true;

		
		if (alwaysAttack)
		{
			if (input.attack)
				input.attack = false;
			else
			{
				input.attack = true;
			}
			
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		//When to set state to stance
		if (!blocker.blocking
			&& combatState != CombatState.Attack
			&& combatState != CombatState.Retract
			&& combatState != CombatState.Windup
			&& combatState != CombatState.Stagger)
		{
			combatState = CombatState.Stance;
		}

		//Turn off collider when not blocking or attacking
		if (combatState == CombatState.Attack
			|| combatState == CombatState.Block
			|| combatState == CombatState.Stagger)
		{
			weapon.EnableCollider(true);
		}
		else
		{
			weapon.EnableCollider(false);
		}

		prevCombatState = combatState;
	}
}