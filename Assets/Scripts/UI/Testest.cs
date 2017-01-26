//using System.Collections;
using UnityEngine;

public class Testest : MonoBehaviour
{
	Rigidbody rb;
	[SerializeField] float someForce = 10f;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
	{
		rb.AddTorque(Vector3.up * someForce * Time.deltaTime);
		print("OK");
	}
}