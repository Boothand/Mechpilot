using System.Collections;
using UnityEngine;

public class Test : MechComponent
{
	[SerializeField] Transform lHandTarget; //Where to put the left hand (if longsword)
	[SerializeField] float armHeight, armDistance = 0.5f;
	[SerializeField] Transform chest;
	[SerializeField] Vector3 rotOffset;
	[SerializeField] float swipeDuration = 0.75f;
	bool inBlockSwipe;
	Vector3 idlePos;
	Quaternion idleRot;
	Vector3 inputVec;
	float inputVecMagnitude;

	public enum SwipeDir { BottomLeft, Left, TopLeft, Top, TopRight, Right, BottomRight }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	SwipeDir GetSwipeDirection(Vector3 inputVec)
	{
		//Top
		if (Mathf.Abs(inputVec.x) < 0.4f && inputVec.y > 0f)
		{
			return SwipeDir.Top;
		}

		//Right
		if (inputVec.x > 0f)
		{
			if (inputVec.y > 0.4f)
			{
				//Top right
				return SwipeDir.TopRight;
			}

			if (inputVec.y < -0.4f)
			{
				//Bottom right
				return SwipeDir.BottomRight;
			}

			return SwipeDir.Right;
		}

		//Left
		if (inputVec.x < 0f)
		{
			if (inputVec.y > 0.4f)
			{
				//Top left
				return SwipeDir.TopLeft;
			}

			if (inputVec.y < -0.4f)
			{
				//Bottom left
				return SwipeDir.BottomLeft;
			}

			return SwipeDir.Left;
		}

		return SwipeDir.Top;
	}

	void SetTargetPosRot(SwipeDir dir, ref Vector3 toPos, ref Quaternion toRot)
	{
		switch (dir)
		{
			case SwipeDir.BottomLeft:
				toPos = arms.armBlockState.lowerLeftTransform.position;
				toRot = arms.armBlockState.lowerLeftTransform.rotation;
				break;

			case SwipeDir.BottomRight:
				toPos = arms.armBlockState.lowerRightTransform.position;
				toRot = arms.armBlockState.lowerRightTransform.rotation;
				break;

			case SwipeDir.Left:
				toPos = arms.armBlockState.leftTransform.position;
				toRot = arms.armBlockState.leftTransform.rotation;
				break;

			case SwipeDir.Right:
				toPos = arms.armBlockState.rightTransform.position;
				toRot = arms.armBlockState.rightTransform.rotation;
				break;

			case SwipeDir.Top:
				toPos = arms.armBlockState.topTransform.position;
				toRot = arms.armBlockState.topTransform.rotation;
				break;

			case SwipeDir.TopLeft:
				toPos = arms.armBlockState.upperLeftTransform.position;
				toRot = arms.armBlockState.upperLeftTransform.rotation;
				break;

			case SwipeDir.TopRight:
				toPos = arms.armBlockState.upperRightTransform.position;
				toRot = arms.armBlockState.upperRightTransform.rotation;
				break;
		}
	}

	IEnumerator Swipe(SwipeDir dir)
	{
		Transform rIK = arms.armControl.getRhandIKTarget;

		Vector3 fromPos = rIK.position;
		Quaternion fromRot = rIK.rotation;
		Vector3 toPos = rIK.position;
		Quaternion toRot = rIK.rotation;

		SetTargetPosRot(dir, ref toPos, ref toRot);

		float timer = 0f;

		while (timer < swipeDuration)
		{
			timer += Time.deltaTime;
			rIK.position = Vector3.Lerp(fromPos, toPos, timer / swipeDuration);
			rIK.rotation = Quaternion.Lerp(fromRot, toRot, timer / swipeDuration);
			yield return null;
		}

		//timer = 0f;
		//fromPos = rIK.position;
		//fromRot = rIK.rotation;
		//toPos = idlePos;
		//toRot = idleRot;

		//while (timer < swipeDuration)
		//{
		//	timer += Time.deltaTime;
		//	toPos = idlePos;
		//	toRot = idleRot;
		//	rIK.position = Vector3.Lerp(fromPos, toPos, timer / swipeDuration);
		//	rIK.rotation = Quaternion.Lerp(fromRot, toRot, timer / swipeDuration);
		//	yield return null;
		//}
		while (inputVecMagnitude > 0.5f)
		{

			yield return null;
		}

		inBlockSwipe = false;
	}

	void Update()
	{
		idlePos = chest.position + Vector3.up * armHeight + chest.forward * armDistance;
		idleRot = chest.rotation * Quaternion.Euler(rotOffset);
		inputVec = new Vector3(input.rArmHorz, input.rArmVert).normalized;
		inputVecMagnitude = inputVec.magnitude;

		if (!inBlockSwipe)
		{
			if (inputVecMagnitude > 0.4f)
			{
				inBlockSwipe = true;

				SwipeDir dir = GetSwipeDirection(inputVec);
				StopAllCoroutines();
				StartCoroutine(Swipe(dir));
			}

			Transform rIK = arms.armControl.getRhandIKTarget;
			rIK.position = Vector3.Lerp(rIK.position, idlePos, Time.deltaTime * 5f);
			rIK.rotation = Quaternion.Lerp(rIK.rotation, idleRot, Time.deltaTime * 5f);
		}

		

		arms.armControl.lHandIKTarget.position = lHandTarget.position;
		arms.armControl.lHandIKTarget.rotation = lHandTarget.rotation;
	}
}