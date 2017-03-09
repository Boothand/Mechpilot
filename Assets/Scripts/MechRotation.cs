//using System.Collections;
using UnityEngine;

public class MechRotation : MechComponent
{
	public Vector3 lookDir { get { return forwardDir;/* aimBaseX.forward;*/ } }
	[SerializeField] float rotationSpeed = 40f;
	float rotationAmount;
	
	Vector3 forwardDir;

	[SerializeField] bool lockOn = true;
	bool lockedOn;

	protected override void OnAwake()
	{
		base.OnAwake();
		lockedOn = true;
	}

	void Update()
	{
		if (lockOn && lockedOn && blocker.tempEnemy &&
			arms.combatState != WeaponsOfficer.CombatState.Attack)
		{
			forwardDir = blocker.tempEnemy.transform.position - mech.transform.position;
		}
		else
		{
			Vector3 targetRot = new Vector3(input.lookHorz, input.lookVert);
			rotationAmount = Mathf.Lerp(rotationAmount, targetRot.x, Time.deltaTime * 4f);

			forwardDir = Quaternion.Euler(0, rotationAmount * rotationSpeed, 0) * mech.transform.forward;
		}

		forwardDir.y = 0f;
		forwardDir.Normalize();
	}
}