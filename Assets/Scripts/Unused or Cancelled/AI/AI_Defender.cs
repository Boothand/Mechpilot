using System.Collections;
using UnityEngine;

public class AI_Defender : AI_MechComponent
{
#if LEGACY

	protected override void OnAwake()
	{
		base.OnAwake();
	}


	protected override void Update()
	{
		base.Update();
		//input.rArmRot = 0f;
		//input.rArmHorz = 0f;
		//input.rArmVert = 0f;

		//switch (aiCombat.blockMethod)
		//{
		//	case AI_Combat.BlockMethod.Confident:

		//		break;

		//	case AI_Combat.BlockMethod.LowHealth:

		//		break;

		//	case AI_Combat.BlockMethod.LowStamina:

		//		break;
		//}
	}
#endif
}