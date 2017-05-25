using System.Collections;
using UnityEngine;

public class IngameUIOverlay : MonoBehaviour
{
	public static IngameUIOverlay instance;


	void Awake()
	{
		if (instance == null)
			instance = this;
	}
}