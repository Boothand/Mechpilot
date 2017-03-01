using System.Collections;
using UnityEngine;

public class Blocker : MechComponent
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
		float threshold = 0.2f;

		//Top
		if (Mathf.Abs(inputVec.x) < threshold && inputVec.y > 0f)
		{
			return SwipeDir.Top;
		}

		//Right
		if (inputVec.x > 0f)
		{
			if (inputVec.y > threshold)
			{
				//Top right
				return SwipeDir.TopRight;
			}

			if (inputVec.y < -threshold)
			{
				//Bottom right
				return SwipeDir.BottomRight;
			}

			return SwipeDir.Right;
		}

		//Left
		if (inputVec.x < 0f)
		{
			if (inputVec.y > threshold)
			{
				//Top left
				return SwipeDir.TopLeft;
			}

			if (inputVec.y < -threshold)
			{
				//Bottom left
				return SwipeDir.BottomLeft;
			}

			return SwipeDir.Left;
		}

		return SwipeDir.Top;
	}

	//Transform GetTargetTransform(SwipeDir dir)
	//{
	//	switch (dir)
	//	{
	//		case SwipeDir.BottomLeft:
	//			return arms.armBlockState.lowerLeftTransform;

	//		case SwipeDir.BottomRight:
	//			return arms.armBlockState.lowerRightTransform;

	//		case SwipeDir.Left:
	//			return arms.armBlockState.leftTransform;

	//		case SwipeDir.Right:
	//			return arms.armBlockState.rightTransform;

	//		case SwipeDir.Top:
	//			return arms.armBlockState.topTransform;

	//		case SwipeDir.TopLeft:
	//			return arms.armBlockState.upperLeftTransform;

	//		case SwipeDir.TopRight:
	//			return arms.armBlockState.upperRightTransform;
	//	}

	//	return arms.armBlockState.topTransform;
	//}

	//IEnumerator Swipe_Old(SwipeDir dir)
	//{
	//	Transform rIK = arms.armControl.getRhandIKTarget;

		

	//	Transform transformToUse = GetTargetTransform(dir);

	//	while (true)
	//	{
	//		float timer = 0f;

	//		Vector3 fromPos = rIK.position;
	//		Quaternion fromRot = rIK.rotation;
	//		Vector3 toPos = transformToUse.position;
	//		Quaternion toRot = transformToUse.rotation;

	//		while (timer < swipeDuration * 0.7f)
	//		{
	//			timer += Time.deltaTime;
	//			rIK.position = Vector3.Lerp(fromPos, toPos, timer / swipeDuration);
	//			rIK.rotation = Quaternion.Lerp(fromRot, toRot, timer / swipeDuration);
	//			yield return null;
	//		}

	//		if (transformToUse.childCount > 0)
	//		{
	//			transformToUse = transformToUse.GetChild(0);
	//		}
	//		else
	//		{
	//			break;
	//		}
	//	}
		

	//	//timer = 0f;
	//	//fromPos = rIK.position;
	//	//fromRot = rIK.rotation;
	//	//toPos = idlePos;
	//	//toRot = idleRot;

	//	//while (timer < swipeDuration)
	//	//{
	//	//	timer += Time.deltaTime;
	//	//	toPos = idlePos;
	//	//	toRot = idleRot;
	//	//	rIK.position = Vector3.Lerp(fromPos, toPos, timer / swipeDuration);
	//	//	rIK.rotation = Quaternion.Lerp(fromRot, toRot, timer / swipeDuration);
	//	//	yield return null;
	//	//}
	//	while (inputVecMagnitude > 0.5f)
	//	{

	//		yield return null;
	//	}

	//	inBlockSwipe = false;
	//}


	IEnumerator Swipe(SwipeDir dir)
	{
		string animToUse = "Top Block";

		switch (dir)
		{
			case SwipeDir.BottomLeft:
				animToUse = "Bottom Left Block";
				break;

			case SwipeDir.BottomRight:
				animToUse = "Bottom Right Block";
				break;

			case SwipeDir.Left:
				animToUse = "Left Block";
				break;

			case SwipeDir.Right:
				animToUse = "Right Block";
				break;

			case SwipeDir.Top:
				animToUse = "Top Block";
				break;

			case SwipeDir.TopLeft:
				animToUse = "Top Left Block";
				break;

			case SwipeDir.TopRight:
				animToUse = "Top Right Block";
				break;
		}

		animator.CrossFade(animToUse, 0.25f, 1);

		yield return new WaitForSeconds(swipeDuration);

		inBlockSwipe = false;
	}

	void Update()
	{
		//idlePos = chest.position + Vector3.up * armHeight + chest.forward * armDistance;
		//idleRot = chest.rotation * Quaternion.Euler(rotOffset);
		inputVec = new Vector3(input.rArmHorz, input.rArmVert).normalized;
		inputVecMagnitude = inputVec.magnitude;

		if (!inBlockSwipe && !attacker.inAttack)
		{
			if (inputVecMagnitude > 0.4f)
			{
				inBlockSwipe = true;

				SwipeDir dir = GetSwipeDirection(inputVec);
				StopAllCoroutines();
				StartCoroutine(Swipe(dir));
			}

			//	Transform rIK = arms.armControl.getRhandIKTarget;
			//	rIK.position = Vector3.Lerp(rIK.position, idlePos, Time.deltaTime * 5f);
			//	rIK.rotation = Quaternion.Lerp(rIK.rotation, idleRot, Time.deltaTime * 5f);
		}



		//arms.armControl.lHandIKTarget.position = lHandTarget.position;
		//arms.armControl.lHandIKTarget.rotation = lHandTarget.rotation;
	}
}