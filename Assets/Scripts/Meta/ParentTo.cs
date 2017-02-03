//using System.Collections;
using UnityEngine;

public class ParentTo : MonoBehaviour
{
	[SerializeField] Transform parentedTo;
	
	void Awake()
	{
		transform.SetParent(parentedTo);
	}
}