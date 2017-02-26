//using System.Collections;
using UnityEngine;

public class ArmWindupState : MechComponent
{
	[Header("Position")]
	[SerializeField] float windupPullBackDistance = 0.19f;

	[Header("Rotation")]
	[SerializeField] float rotateBackAmount = 24;
	[SerializeField] float windupRotSpeed = 2.5f;

	[SerializeField] Transform centerTransform, lowerLeftTransform, leftTransform, upperLeftTransform, topTransform,
								upperRightTransform, rightTransform, lowerRightTransform, bottomTransform;

	public float getWindupRotSpeed { get { return windupRotSpeed; } }
	public float getWindupPullBackDistance { get { return windupPullBackDistance; } }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public Quaternion WindUpRotation()
	{
		Quaternion verticalAngle = Quaternion.Euler(0, -rotateBackAmount, 0);

		Vector3 rArmPos = arms.armBlockState.rArmPos;

		Quaternion rotToUse = Quaternion.identity;
		if (rArmPos.x >= 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top right
				rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, rightTransform.rotation,
																topTransform.rotation, upperRightTransform.rotation);
			}
			if (rArmPos.y < 0f)
			{
				//Bottom right
				rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, rightTransform.rotation,
																				bottomTransform.rotation, lowerRightTransform.rotation);
			}
		}
		else if (rArmPos.x < 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top left
				rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, leftTransform.rotation,
																topTransform.rotation, upperLeftTransform.rotation);
			}
			if (rArmPos.y < 0f)
			{
				//Bottom left
				rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, leftTransform.rotation,
																			bottomTransform.rotation, lowerLeftTransform.rotation);
			}
		}

		//return arms.armControl.handSideRotation * verticalAngle;
		return rotToUse;
	}

	public Vector3 WindUpPosition()
	{
		Vector3 blockPos = arms.armControl.blockPos;
		Vector3 dir = -mech.transform.forward;
		float length = arms.armWindupState.getWindupPullBackDistance * scaleFactor;

		Vector3 rArmPos = arms.armBlockState.rArmPos;
		Vector3 posToUse = Vector3.zero;

		if (rArmPos.x >= 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top right
				posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, rightTransform.position,
																topTransform.position, upperRightTransform.position);
			}
			if (rArmPos.y < 0f)
			{
				//Bottom right
				posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, rightTransform.position,
																				bottomTransform.position, lowerRightTransform.position);
			}
		}
		else if (rArmPos.x < 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top left
				posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, leftTransform.position,
																topTransform.position, upperLeftTransform.position);
			}
			if (rArmPos.y < 0f)
			{
				//Bottom left
				posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, leftTransform.position,
																			bottomTransform.position, lowerLeftTransform.position);
			}
		}

		//return blockPos + dir * length;
		return posToUse;
	}
}