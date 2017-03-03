//using System.Collections;
using UnityEngine;

public class StancePicker : MechComponent
{
	[SerializeField] Transform trTransform, tlTransform, brTransform, blTransform, topTransform;
	public WeaponsOfficer.CombatDir stance { get; private set; }

	Transform targetTransform;

	[SerializeField] float blendSpeed = 4f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	

	void SetStanceTransform()
	{
		switch (stance)
		{
			case WeaponsOfficer.CombatDir.BottomLeft:
				targetTransform = blTransform;
				break;

			case WeaponsOfficer.CombatDir.BottomRight:
				targetTransform = brTransform;
				break;

			case WeaponsOfficer.CombatDir.Top:
				targetTransform = topTransform;
				break;

			case WeaponsOfficer.CombatDir.TopLeft:
				targetTransform = tlTransform;
				break;

			case WeaponsOfficer.CombatDir.TopRight:
				targetTransform = trTransform;
				break;
		}
	}

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Stance)
		{
			stance = arms.DecideCombatDir(stance);

			SetStanceTransform();

			Transform rIK = arms.armControl.getRhandIKTarget;

			rIK.position = Vector3.Lerp(rIK.position, targetTransform.position, Time.deltaTime * blendSpeed);
			rIK.rotation = Quaternion.Lerp(rIK.rotation, targetTransform.rotation, Time.deltaTime * blendSpeed);
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}