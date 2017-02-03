//using System.Collections;
using UnityEngine;

public class ArmStaggerState : MechComponent
{
	[SerializeField] float staggerBeginRotSpeed = 3f;
	[SerializeField] float staggerEndRotSpeed = 3f;

	[SerializeField] float blockStaggerBeginRotSpeed = 6f;
	[SerializeField] float blockStaggerEndRotSpeed = 6f;

	public float getStaggerBeginRotSpeed { get { return staggerBeginRotSpeed; } }
	public float getStaggerEndRotSpeed { get { return staggerEndRotSpeed; } }

	public float getBlockStaggerBeginRotSpeed { get { return blockStaggerBeginRotSpeed; } }
	public float getBlockStaggerEndRotSpeed { get { return blockStaggerEndRotSpeed; } }

	protected override void OnAwake()
	{
		base.OnAwake();
	}


	public Quaternion StaggerRotation(Vector3 otherVelocity, float multiplier)
	{
		Vector3 swordVelocity = arms.getWeapon.swordTipVelocity;
		Vector3 swordTipPos = arms.getWeapon.getSwordTip.position;
		
		Vector3 newTipPos = swordTipPos + otherVelocity * multiplier;

		//Vector from hand to new sword tip position
		Vector3 newSwordDir = (newTipPos - arms.armControl.getRhandIKTarget.position).normalized;

		//The world rotation of the new sword vector
		Quaternion newWorldRot = Quaternion.LookRotation(newSwordDir, mech.transform.forward);

		//Transform to local space
		Quaternion newLocalRot = Quaternion.Inverse(mech.transform.rotation) * newWorldRot;

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
}