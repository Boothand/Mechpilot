using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using RootMotion.Dynamics;

//A sort of management class for the weapons' officer
public class WeaponsOfficer : MechComponent
{
	//Components specific to him:
	public Blocker blocker { get; private set; }
	public Attacker attacker { get; private set; }
	public StancePicker stancePicker { get; private set; }
	public Windup windup { get; private set; }
	public Retract retract { get; private set; }
	public Stagger stagger { get; private set; }

	//States:
	public enum CombatState { Stance, Block, Windup, Attack, Stagger, Retract }
	public CombatState combatState;
	public CombatState prevCombatState;

	//Swing directions:
	public enum CombatDir { TopRight, TopLeft, /*BottomRight, BottomLeft,*/ Top }

	public Vector3 inputVec { get; private set; }
	public float inputVecMagnitude { get; private set; }

	//Plugin stuff:
	public FullBodyBipedIK fbbik { get; private set; }
	public PuppetMaster puppet { get; private set; }

	[SerializeField] Sword weapon;
	public Sword getWeapon { get { return weapon; } }

	//Debug:
	[SerializeField] bool alwaysBlock;
	[SerializeField] bool alwaysAttack;


	protected override void OnAwake()
	{
		base.OnAwake();
		
		fbbik = transform.root.GetComponentInChildren<FullBodyBipedIK>();
		puppet = transform.root.GetComponentInChildren<PuppetMaster>();
		blocker = mech.transform.root.GetComponentInChildren<Blocker>();
		attacker = mech.transform.root.GetComponentInChildren<Attacker>();
		stancePicker = mech.transform.root.GetComponentInChildren<StancePicker>();
		windup = mech.transform.root.GetComponentInChildren<Windup>();
		retract = mech.transform.root.GetComponentInChildren<Retract>();
		stagger = mech.transform.root.GetComponentInChildren<Stagger>();
	}

	void Start()
	{
		//IgnoreHierarchyRecursive(transform.root, weapon.GetComponent<Collider>());
		StartCoroutine(LockWeaponMotionRoutine());	//Hack: Lock it one frame after start, to override puppetmaster?
	}

	IEnumerator LockWeaponMotionRoutine()
	{
		yield return null;
		weapon.LockSwordAngularMotion(true);
	}

	IEnumerator TweenLayerWeightRoutine(float weight, int layer, float time)
	{
		float timer = 0f;
		float fromWeight = animator.GetLayerWeight(layer);

		while (timer < time)
		{
			timer += Time.deltaTime;

			animator.SetLayerWeight(layer, Mathf.Lerp(fromWeight, weight, timer / time));
			yield return null;
		}

		animator.SetLayerWeight(layer, weight);
	}

	public void TweenLayerWeight(float weight, int layer, float time)
	{
		StartCoroutine(TweenLayerWeightRoutine(weight, layer, time));
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

	//Returns a direction according to right stick input:
	public CombatDir DecideCombatDir(CombatDir inDir)
	{
		if (Mathf.Abs(inputVec.x) < 0.4f &&
			inputVec.y > 0.4f)
		{
			return CombatDir.Top;
		}

		if (inputVec.x > 0.1f)
		{
			//if (inputVec.y < -0f)
			//{
			//	//Bottom right
			//	return WeaponsOfficer.CombatDir.BottomRight;
			//}

			//Top right
			return CombatDir.TopRight;
		}

		if (inputVec.x < -0.1f)
		{
			//if (inputVec.y < -0f)
			//{
			//	//Bottom left
			//	return WeaponsOfficer.CombatDir.BottomLeft;
			//}

			//Top left
			return CombatDir.TopLeft;
		}

		//if (inputVec.y < -0.1f && Mathf.Abs(inputVec.x) < 0.1f)
		//{
		//	if (inDir == CombatDir.TopLeft)
		//		return CombatDir.BottomLeft;

		//	if (inDir == CombatDir.TopRight)
		//		return CombatDir.BottomRight;
		//}

		//Default
		return inDir;
	}

	IEnumerator SetPinWeightWholeBodyRoutine(float fromWeight, float toWeight, float time)
	{
		float timer = 0f;

		while (timer < time)
		{
			timer += Time.deltaTime;

			puppet.pinWeight = Mathf.Lerp(fromWeight, toWeight, timer / time);

			yield return null;
		}

		puppet.pinWeight = toWeight;
	}

	public void SetPinWeightWholeBody(float fromWeight, float toWeight, float time)
	{
		StartCoroutine(SetPinWeightWholeBodyRoutine(fromWeight, toWeight, time));
	}

	public void KillPuppet()
	{
		//PuppetMaster.StateSettings settings = new PuppetMaster.StateSettings(1f, 0.01f, 2f, 0.02f, false, true, true);
		//puppet.Kill(settings);

		puppet.muscleWeight = 0.1f;
		puppet.pinWeight = 0f;
	}

	protected override void OnUpdate ()
	{
		//So others can get them without calculating it potentially several times:
		inputVec = new Vector3(input.rArmHorz, input.rArmVert).normalized;
		inputVecMagnitude = inputVec.magnitude;

		//Debug:
		if (alwaysBlock)
			input.block = true;
		
		//Debug:
		if (alwaysAttack)
		{
			if (input.attack)
			{
				input.attack = false;
			}
			else
			{
				input.attack = true;
			}
		}

		//Debug:
		if (Input.GetKeyDown(KeyCode.R))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}


		prevCombatState = combatState;
	}
}