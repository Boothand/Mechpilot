//using System.Collections;
using UnityEngine;

public class AI_Combat : MechComponent
{
	[SerializeField] float combatDistance = 45f;
	public enum CombatState { Defend, Attack }
	public CombatState combatState { get; private set; }

	public float getCombatDistance { get { return combatDistance; } }

	public Vector3 localHandBasePos { get; private set; }


	protected override void OnAwake()
	{
		base.OnAwake();

		combatState = CombatState.Defend;
	}

	public void MoveHandsToPos(Vector3 localPos)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

		Vector3 dir = localPos - rIK.localPosition;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	public void MoveHandsInDirection(Vector3 dir)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

		Vector3 direction = dir;

		input.rArmHorz = Mathf.Sign(dir.x);
		input.rArmVert = Mathf.Sign(dir.y);
	}

	public void RotateHandsToAngle(float angle)
	{
		float aimAngle = arms.armBlockState.sideTargetAngle;

		if (aimAngle < angle - 0.01f)
		{
			input.rArmRot = 1f;
		}
		else if (aimAngle > angle + 0.01f)
		{
			input.rArmRot = -1f;
		}
	}

	public void StopArms()
	{
		input.rArmHorz = 0f;
		input.rArmVert = 0f;
	}

	public void StopHandRotation()
	{
		input.rArmRot = 0f;
	}


	void Update()
	{
		//Decide which combat state to use
		Vector3 basePos = arms.armControl.handCenterPos;
		localHandBasePos = mech.transform.InverseTransformPoint(basePos);
	}
}