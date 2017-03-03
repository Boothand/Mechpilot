//using System.Collections;
using UnityEngine;

public class ArmBlockState : MechComponent
{
	[Header("Position")]
	[SerializeField] float idleMoveSpeed = 1.25f;
	[Range(-1f, 1f)] [SerializeField] float armHeight = 0.2f;
	public float getArmHeight { get { return armHeight; } }
	[Range(0.05f, 1f)] [SerializeField] float rArmDistance = 0.445f;
	[Range(0.05f, 1f)] [SerializeField] float lArmDistance = 0.232f;
	[Range(0.2f, 2f)] [SerializeField] float armReach = 0.38f;
	bool movingArm;
	public Vector3 rArmPos { get; private set; }
	public Vector3 lArmPos { get; private set; }

	[Header("Rotation")]
	[SerializeField] bool invertRotation;
	public float sideTargetAngle { get; private set; }
	[SerializeField] float rotationSpeed = 250f;
	[SerializeField] float sideRotationLimit = 125f;
	public float getSideRotationLimit { get { return sideRotationLimit; } }
	[SerializeField] Vector3 sideRotOffset;

	[SerializeField] public Transform centerTransform, lowerLeftTransform, leftTransform, upperLeftTransform, topTransform,
								upperRightTransform, rightTransform, lowerRightTransform, bottomTransform;

	[SerializeField] public Transform hangCenterTransform, hangLowerLeftTransform, hangLeftTransform, hangUpperLeftTransform, hangTopTransform,
								hangUpperRightTransform, hangRightTransform, hangLowerRightTransform, hangBottomTransform;
	
	public delegate void ArmMovement();
	public event ArmMovement OnMoveArmBegin;
	public event ArmMovement OnMoveArm;
	public event ArmMovement OnMoveArmEnd;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public Vector3 BlockPos()
	{
		ArmControl.State state = arms.armControl.state;

		Vector3 inputVec = new Vector3(input.rArmHorz, input.rArmVert);
		//Vector3 lMoveInput = new Vector3(input.lArmHorz, input.lArmVert);   //Only for shield

		Vector3 actualArmPos;

		//Limit max input delta
		inputVec = Vector3.ClampMagnitude(inputVec, 1f);

		float speedToUse = idleMoveSpeed;

		if (state == ArmControl.State.Staggered ||
			state == ArmControl.State.StaggeredEnd)
		{
			speedToUse = idleMoveSpeed / 1.75f;
		}

		//Add input values to XY position
		rArmPos += inputVec * speedToUse * Time.deltaTime * scaleFactor;

		float inputVecMagnitude = inputVec.magnitude;

		#region Event Stuff
		if (!movingArm && inputVecMagnitude > 0.1f)
		{
			movingArm = true;
			if (OnMoveArmBegin != null) { OnMoveArmBegin(); }
		}

		if (movingArm && inputVecMagnitude > 0.1f)
		{
			if (OnMoveArm != null) { OnMoveArm(); }
		}

		if (movingArm && inputVecMagnitude < 0.1f)
		{
			movingArm = false;
			if (OnMoveArmEnd != null) { OnMoveArmEnd(); }
		}
		#endregion

		//Limit arm's reach on local XY axis
		//rArmPos = Vector3.ClampMagnitude(rArmPos, armReach * scaleFactor);
		rArmPos = new Vector3(Mathf.Clamp(rArmPos.x, -12f, 12f), Mathf.Clamp(rArmPos.y, -12f, 12f));

		Vector3 posToUse = Vector3.zero;

		if (rArmPos.x >= 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top right
				if (!input.block)
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, rightTransform.position,
																	topTransform.position, upperRightTransform.position);
				}
				else
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.position, hangRightTransform.position,
															hangTopTransform.position, hangUpperRightTransform.position);
				}
			}
			if (rArmPos.y < 0f)
			{
				//Bottom right
				if (!input.block)
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, rightTransform.position,
																					bottomTransform.position, lowerRightTransform.position);
				}
				else
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.position, hangRightTransform.position,
																					hangBottomTransform.position, hangLowerRightTransform.position);
				}
			}
		}
		else if (rArmPos.x < 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top left
				if (!input.block)
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, leftTransform.position,
																	topTransform.position, upperLeftTransform.position);
				}
				else
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.position, hangLeftTransform.position,
																	hangTopTransform.position, hangUpperLeftTransform.position);
				}
			}
			if (rArmPos.y < 0f)
			{
				//Bottom left
				if (!input.block)
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, centerTransform.position, leftTransform.position,
																				bottomTransform.position, lowerLeftTransform.position);
				}
				else
				{
					posToUse = BlendTree2D.BlendedPos(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.position, hangLeftTransform.position,
																			hangBottomTransform.position, hangLowerLeftTransform.position);
				}
			}
		}
		actualArmPos = mech.transform.TransformDirection(rArmPos);

		//The center of the circular area used for the arm movement
		//Vector3 handCentralPos = shoulder.position + Vector3.up * armHeight * scaleFactor;
		Vector3 handCentralPos = arms.armControl.handCenterPos;

		//print(rArmPos);
		//animator.SetFloat("Hand Pos X", rArmPos.x);

		//Dirty check to see which shoulder is used, and what arm distance to use.
		float armDistance = rArmDistance;

		//if (shoulder == hierarchy.lShoulder)
		//{
		//	armDistance = lArmDistance;
		//}

		//Set hand position on local Z axis
		handCentralPos += mech.transform.forward * armDistance * scaleFactor;

		//Final position
		//return handCentralPos + actualArmPos;
		return posToUse;
	}

	public Quaternion ArmSideRotation()
	{
		float rotationInput = Mathf.Clamp(input.rArmRot, -1f, 1f);
		ArmControl.State state = arms.armControl.state;
		//Add input values to the target rotation
		int factor = 1;

		if (invertRotation)
			factor = -1;

		if (state != ArmControl.State.Attack &&
			state != ArmControl.State.Staggered)
		{
			sideTargetAngle += factor * rotationInput * Time.deltaTime * rotationSpeed;
		}

		//Wrap
		if (sideTargetAngle > 360)
			sideTargetAngle -= 360f;

		if (sideTargetAngle < -360)
			sideTargetAngle += 360f;

		//Limit rotation angle based on state
		float sideRotLimitToUse = sideRotationLimit;

		switch (arms.armControl.state)
		{
			case ArmControl.State.WindUp:
			case ArmControl.State.WindedUp:
			case ArmControl.State.Attack:
				sideRotLimitToUse = 100f;
				break;

		}

		//Limit target rotation
		sideTargetAngle = Mathf.Clamp(sideTargetAngle, -sideRotLimitToUse, sideRotLimitToUse);

		//Return the rotation
		Quaternion offset = Quaternion.Euler(sideRotOffset);
		Quaternion localRotation = offset * Quaternion.Euler(-sideTargetAngle, 0, 0);

		Quaternion rotToUse = Quaternion.identity;
		if (rArmPos.x >= 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top right
				if (!input.block)
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, rightTransform.rotation,
																	topTransform.rotation, upperRightTransform.rotation);
				}
				else
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.rotation, hangRightTransform.rotation,
																hangTopTransform.rotation, hangUpperRightTransform.rotation);
				}
			}
			if (rArmPos.y < 0f)
			{
				//Bottom right
				if (!input.block)
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, rightTransform.rotation,
																					bottomTransform.rotation, lowerRightTransform.rotation);
				}
				else
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.rotation, hangRightTransform.rotation,
																					hangBottomTransform.rotation, hangLowerRightTransform.rotation);
				}
			}
		}
		else if (rArmPos.x < 0f)
		{
			if (rArmPos.y >= 0f)
			{
				//Top left
				if (!input.block)
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, leftTransform.rotation,
																	topTransform.rotation, upperLeftTransform.rotation);
				}
				else
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.rotation, hangLeftTransform.rotation,
																hangTopTransform.rotation, hangUpperLeftTransform.rotation);
				}
			}
			if (rArmPos.y < 0f)
			{
				//Bottom left
				if (!input.block)
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, centerTransform.rotation, leftTransform.rotation,
																				bottomTransform.rotation, lowerLeftTransform.rotation);
				}
				else
				{
					rotToUse = BlendTree2D.BlendedRot(rArmPos.x / 12, rArmPos.y / 12, hangCenterTransform.rotation, hangLeftTransform.rotation,
																			hangBottomTransform.rotation, hangLowerLeftTransform.rotation);
				}
			}
		}

		//return localRotation;
		return rotToUse;
	}
}