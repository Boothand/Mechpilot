//using System.Collections;
using UnityEngine;

public class XYMover : MonoBehaviour
{
	[SerializeField]
	Transform posBase;

	[SerializeField]
	Vector3 offset;

	[SerializeField]
	string inputXStr = "Mouse X";

	[SerializeField]
	string inputYStr = "Mouse Y";

	[SerializeField]
	float maxInputSpeed = 0.25f;

	[SerializeField]
	float speed = 1f;

	[SerializeField]
	bool clampMagnitude;

	[SerializeField]
	float maxMagnitude = 1f;

	Vector3 inputOffset;


	void Start ()
	{

	}
	
	void Update ()
	{
		float inputX = Input.GetAxis(inputXStr);
		float inputY = Input.GetAxis(inputYStr);
		
		//Limit input values
		inputX = Mathf.Clamp(inputX, -maxInputSpeed, maxInputSpeed);
		inputY = Mathf.Clamp(inputY, -maxInputSpeed, maxInputSpeed);

		inputOffset.x += inputX * speed * Time.deltaTime;
		inputOffset.y += inputY * speed * Time.deltaTime;

		Vector3 offsetDir =
			(posBase.right * offset.x) +
			(posBase.up * offset.y) +
			(posBase.forward * offset.z);

		Vector3 inputOffsetDir =
			(posBase.right * inputOffset.x) +
			(posBase.up * inputOffset.y) +
			(posBase.forward * inputOffset.z);

		//Don't go outside a circular area
		if (clampMagnitude)
		{
			inputOffsetDir = Vector3.ClampMagnitude(inputOffsetDir, maxMagnitude);
			inputOffset = Vector3.ClampMagnitude(inputOffset, maxMagnitude);
		}
		

		Vector3 finalPosition = posBase.position + offsetDir + inputOffsetDir;

		transform.position = finalPosition;		
	}
}