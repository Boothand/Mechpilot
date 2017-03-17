using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
{
	[SerializeField] IKPose trTransform, tlTransform, brTransform, blTransform, topTransform;
	[SerializeField] IKPose bl2br, bl2bl, br2br;
	IKPose targetPose;
	//Quaternion targetPosOffset;
	public Quaternion targetRotOffset;
	WeaponsOfficer.CombatDir blockStance;
	WeaponsOfficer.CombatDir idealBlock;
	WeaponsOfficer.CombatDir prevBlockStance;

	[SerializeField] float minBlockTime = 0.5f;

	[SerializeField] float blockDuration = 0.75f;

	[SerializeField] bool autoBlock;
	public bool blocking { get; private set; }
	bool switchingBlockStance;

	public Mech tempEnemy;
	Coroutine blockRoutine;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		arms.getWeapon.OnCollision -= OnSwordCollision;
		arms.getWeapon.OnCollision += OnSwordCollision;
	}

	void OnSwordCollision(Collision col)
	{
		Sword otherSword = col.transform.GetComponent<Sword>();
		if (otherSword && otherSword.arms.prevCombatState == WeaponsOfficer.CombatState.Attack)
		{
			//If I block the other
			if (arms.combatState == WeaponsOfficer.CombatState.Block)
			{
				StartCoroutine(CheckCounterAttackRoutine());
			}
		}
	}

	IEnumerator CheckCounterAttackRoutine()
	{
		float timer = 0f;

		while (timer < 0.5f)
		{
			timer += Time.deltaTime;

			//if (input.attack)
			//{
			//	StopAllCoroutines();
			//	blocking = false;
			//	attacker.AttackInstantly(blockStance);
			//	break;
			//}

			yield return null;
		}
	}

	public void Stop()
	{
		StopAllCoroutines();
		blocking = false;
	}

	IKPose GetTargetPose(WeaponsOfficer.CombatDir dir)
	{
		switch (dir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return blTransform;

			case WeaponsOfficer.CombatDir.BottomRight:
				return brTransform;

			case WeaponsOfficer.CombatDir.Top:
				return topTransform;

			case WeaponsOfficer.CombatDir.TopLeft:
				return tlTransform;

			case WeaponsOfficer.CombatDir.TopRight:
				return trTransform;
		}

		return topTransform;
	}

	WeaponsOfficer.CombatDir DecideBlockStance(WeaponsOfficer.CombatDir enemyAttackDir)
	{
		switch (enemyAttackDir)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				return WeaponsOfficer.CombatDir.BottomRight;
			case WeaponsOfficer.CombatDir.BottomRight:
				return WeaponsOfficer.CombatDir.BottomLeft;
			case WeaponsOfficer.CombatDir.Top:
				return WeaponsOfficer.CombatDir.Top;
			case WeaponsOfficer.CombatDir.TopLeft:
				return WeaponsOfficer.CombatDir.TopRight;
			case WeaponsOfficer.CombatDir.TopRight:
				return WeaponsOfficer.CombatDir.TopLeft;
		}

		return WeaponsOfficer.CombatDir.Top;
	}

	//void AdjustPosition()
	//{
	//	Transform rIK = arms.getRhandIKTarget;

	//	Vector3 myMidPoint = arms.getWeapon.getMidPoint.position;
	//	Vector3 otherMidPoint = tempEnemy.weaponsOfficer.getWeapon.getMidPoint.position;

		
	//	if (tempEnemy.weaponsOfficer.combatState == WeaponsOfficer.CombatState.Attack &&
	//		blockStance == idealBlock)
	//	{
	//		//Up/down
	//		if (myMidPoint.y < otherMidPoint.y)
	//		{
	//			targetPosOffset += Vector3.up * Time.deltaTime * 2f;
	//		}
	//		else if (myMidPoint.y > otherMidPoint.y)
	//		{
	//			targetPosOffset -= Vector3.up * Time.deltaTime * 2f;
	//		}

	//		Vector3 localMyPoint = mech.transform.InverseTransformPoint(myMidPoint);
	//		Vector3 localOtherPoint = mech.transform.InverseTransformPoint(otherMidPoint);
	//		//tempEnemy.weaponsOfficer.getWeapon.getSwordTip

	//		//Left/right
	//		if (localMyPoint.x < localOtherPoint.x)
	//		{
	//			targetPosOffset += mech.transform.right * Time.deltaTime * 2f;
	//			//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(50f, 0, 0f);
	//		}
	//		else if (localMyPoint.x > localOtherPoint.x)
	//		{
	//			targetPosOffset -= mech.transform.right * Time.deltaTime * 2f;
	//			//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(-50f, 0f, 0f);
	//		}

	//		//Forward/back
	//		if (localMyPoint.z < localOtherPoint.z)
	//		{
	//			targetPosOffset += mech.transform.forward * Time.deltaTime * 2f;
	//			//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(50f, 0, 0f);
	//		}
	//		else if (localMyPoint.z > localOtherPoint.z)
	//		{
	//			targetPosOffset -= mech.transform.forward * Time.deltaTime * 2f;
	//			//targetRotOffset *= Quaternion.Inverse(mech.transform.rotation) * Quaternion.Euler(-50f, 0f, 0f);
	//		}
	//	}
	//	else
	//	{
	//		targetPosOffset = Vector3.Lerp(targetPosOffset, Vector3.zero, Time.deltaTime * 3f);
	//		targetRotOffset = Quaternion.Lerp(targetRotOffset, Quaternion.identity, Time.deltaTime * 3f);
	//	}
	//}

	IKPose GetTransitionStance(WeaponsOfficer.CombatDir prev, WeaponsOfficer.CombatDir current)
	{
		if (prev == WeaponsOfficer.CombatDir.BottomLeft && current == WeaponsOfficer.CombatDir.BottomRight)
			return bl2br;

		if (prev == WeaponsOfficer.CombatDir.BottomRight && current == WeaponsOfficer.CombatDir.BottomLeft)
			return bl2br;

		if (stancePicker.prevStance == WeaponsOfficer.CombatDir.BottomLeft && current == WeaponsOfficer.CombatDir.BottomLeft)
			return bl2bl;

		if (stancePicker.prevStance == WeaponsOfficer.CombatDir.BottomRight && current == WeaponsOfficer.CombatDir.BottomRight)
			return br2br;

		if (prev == WeaponsOfficer.CombatDir.Top && current == WeaponsOfficer.CombatDir.BottomLeft)
			return bl2br;

		if (prev == WeaponsOfficer.CombatDir.BottomLeft && current == WeaponsOfficer.CombatDir.Top)
			return bl2br;

		return null;
	}

	IEnumerator BlockRoutine()
	{
		switchingBlockStance = true;
		IKPose midPose = stancePicker.bottomMidPose;

		//Find out whether to use a middle pose or not
		midPose = GetTransitionStance(prevBlockStance, blockStance);

		prevBlockStance = blockStance;
		float durationToUse = blockDuration;

		if (midPose != null)
		{
			durationToUse /= 2;
			arms.StoreTargets();

			float timer2 = 0f;

			while (timer2 < durationToUse)
			{
				timer2 += Time.deltaTime;
				arms.InterpolateIKPose(midPose, timer2 / durationToUse);
				yield return null;
			}
		}


		arms.StoreTargets();
		targetPose = GetTargetPose(blockStance);

		float timer = 0f;

		while (timer < durationToUse)
		{
			timer += Time.deltaTime;

			arms.InterpolateIKPose(targetPose, timer / durationToUse);
			yield return null;
		}

		switchingBlockStance = false;
		stancePicker.ForceStance(blockStance);

	}

	IEnumerator BlockTimingRoutine()
	{
		yield return new WaitForSeconds(minBlockTime);

		while (input.block
			|| switchingBlockStance)
		{
			yield return null;
		}

		blocking = false;
		arms.combatState = WeaponsOfficer.CombatState.Stance;
	}

	void Update()
	{
		//Check if prev block state == block state.
		//Start changing block stance:
			//Check if it's necessary to go via another stance.
			//Do so in half the time

		//Initiate the block
		if (!blocking && input.block)
		{
			blocking = true;

			stancePicker.Stop();
			windup.Stop();
			attacker.Stop();
			retract.Stop();
			stagger.Stop();
			arms.combatState = WeaponsOfficer.CombatState.Block;

			//Check when we are allowed to stop blocking:
			StartCoroutine(BlockTimingRoutine());

			blockStance = stancePicker.stance;
			//Enter the block pose:
			if (blockRoutine != null)
			{
				StopCoroutine(blockRoutine);
			}

			blockRoutine = StartCoroutine(BlockRoutine());
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Block)
		{
			if (autoBlock)
			{
				idealBlock = DecideBlockStance(tempEnemy.weaponsOfficer.attacker.dir);
				blockStance = idealBlock;
			}
			else
			{
				blockStance = stancePicker.stance;
			}

			if (prevBlockStance != blockStance)
			{
				//Enter the block pose:
				if (blockRoutine != null)
				{
					StopCoroutine(blockRoutine);
				}

				blockRoutine = StartCoroutine(BlockRoutine());
			}

			if (!switchingBlockStance)
			{
				arms.StoreTargets();
				arms.InterpolateIKPose2(targetPose, targetRotOffset, Time.deltaTime * 4f);
			}

			//targetPose = GetTargetPose(blockStance);

			//AdjustPosition();

			//Only for the sake of maintaining crouch height atm

		}
	}
}