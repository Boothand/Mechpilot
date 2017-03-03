//using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	public WeaponsOfficer.CombatDir stance { get; private set; }

	public Transform targetTransform { get; private set; }

	[SerializeField] float blendSpeed = 4f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	

	public Transform GetStanceTransform()
	{
		switch (stance)
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

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			stance = arms.DecideCombatDir(stance);

			targetTransform = GetStanceTransform();

			Transform rIK = arms.getRhandIKTarget;

			rIK.position = Vector3.Lerp(rIK.position, targetTransform.position, Time.deltaTime * blendSpeed);
			rIK.rotation = Quaternion.Lerp(rIK.rotation, targetTransform.rotation, Time.deltaTime * blendSpeed);
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}