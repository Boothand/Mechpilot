using UnityEngine;
using System.Collections;

public class Engineer : MechComponent
{
	public float legsEnergy { get; private set; }
	public float armsEnergy { get; private set; }
	public float engineerEnergy { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		legsEnergy = 1f;
		armsEnergy = 1f;
		engineerEnergy = 1f;
	}
	
	void Update ()
	{
		
	}
}