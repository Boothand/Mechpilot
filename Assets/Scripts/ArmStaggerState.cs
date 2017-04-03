//using System.Collections;
using UnityEngine;

public class ArmStaggerState : MechComponent
{
#if LEGACY
	[SerializeField] float staggerBeginRotSpeed = 3f;
	[SerializeField] float staggerEndRotSpeed = 3f;

	[SerializeField] float blockBeginRotSpeed = 6f;
	[SerializeField] float blockEndRotSpeed = 6f;

	[SerializeField] float blockMultiplier = 0.08f;
	[SerializeField] float clashMultiplier = 0.12f;
	[SerializeField] float blockedMultiplier = 0.25f;

	public float getBlockMultiplier { get { return blockMultiplier; } }
	public float getBlockedMultiplier { get { return blockedMultiplier; } }
	public float getClashMultiplier { get { return clashMultiplier; } }

	public float getStaggerBeginRotSpeed { get { return staggerBeginRotSpeed; } }
	public float getStaggerEndRotSpeed { get { return staggerEndRotSpeed; } }

	public float getBlockStaggerBeginRotSpeed { get { return blockBeginRotSpeed; } }
	public float getBlockStaggerEndRotSpeed { get { return blockEndRotSpeed; } }

	protected override void OnAwake()
	{
		base.OnAwake();
	}


	public Quaternion StaggerRotation(Vector3 otherVelocity, float multiplier)
	{
		Vector3 swordVelocity = arms.getWeapon.swordTipVelocity;
		Vector3 swordTipPos = arms.getWeapon.getSwordTip.position;
		Vector3 combinedVelocity = -swordVelocity + otherVelocity;
		Debug.DrawRay(swordTipPos, swordVelocity, Color.red);
		Vector3 newTipPos = swordTipPos + combinedVelocity * multiplier;

		//Vector from hand to new sword tip position
		Vector3 newSwordDir = (newTipPos - arms.getRhandIKTarget.position).normalized;

		//The world rotation of the new sword vector
		Quaternion newWorldRot = Quaternion.LookRotation(newSwordDir, mech.transform.forward);

		//Transform to local space
		Quaternion newLocalRot = Quaternion.Inverse(mech.transform.rotation) * newWorldRot * Quaternion.Euler(0, 0, 90);

		//Debug.DrawLine(arms.getWeapon.getSwordTip.position, newTipPos, Color.black);


		//Debug.DrawLine(arms.getWeapon.getSwordTip.position, newTipPos, Color.black);
		return newLocalRot;
	}

	public Vector3 StaggerPosition(Sword otherSword, float multiplier)
	{
		Vector3 otherVelocity = otherSword.swordTipVelocity;
		return arms.armControl.blockPos + otherVelocity * multiplier;
	}

	void Update()
	{
		
	}
#endif
}