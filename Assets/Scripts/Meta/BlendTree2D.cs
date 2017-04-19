using System.Collections.Generic;
using UnityEngine;

public static class BlendTree2D
{
	struct BlendPoint
	{
		public Vector3 pos;
		public float x, y;

		public BlendPoint(Vector3 pos, float x, float y)
		{
			this.pos = pos;
			this.x = x;
			this.y = y;
		}
	}

	static List<BlendPoint> pointList = new List<BlendPoint>();

	public static void Add(Vector3 pos, float x, float y)
	{
		BlendPoint point = new BlendPoint(pos, x, y);

		pointList.Add(point);
	}

	//public static Vector3 Get(float x, float y)
	//{
	//	bool top = true;
	//	bool right = true;
	//}

	public static Vector3 BlendedPos(float x, float y, Vector3 bottom1, Vector3 bottom2, Vector3 top1, Vector3 top2)
	{
		Vector3 bottomRow = Vector3.Lerp(bottom1, bottom2, Mathf.Abs(x));
		Vector3 topRow = Vector3.Lerp(top1, top2, Mathf.Abs(x));

		Vector3 blendBetweenRows = Vector3.Lerp(bottomRow, topRow, Mathf.Abs(y));

		return blendBetweenRows;
	}

	public static Quaternion BlendedRot(float x, float y, Quaternion bottom1, Quaternion bottom2, Quaternion top1, Quaternion top2)
	{
		Quaternion bottomRow = Quaternion.Lerp(bottom1, bottom2, Mathf.Abs(x));
		Quaternion topRow = Quaternion.Lerp(top1, top2, Mathf.Abs(x));

		Quaternion blendBetweenRows = Quaternion.Lerp(bottomRow, topRow, Mathf.Abs(y));

		return blendBetweenRows;
	}
}