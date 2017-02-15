//using System.Collections;
using UnityEngine;

public class AI_Defender : AI_MechComponent
{
	[SerializeField] float posUnderEnemyArmPos = 10f;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void CrossEnemySwordDir(float rotDir = 1f)
	{
		Sword enemySword = enemy.weaponsOfficer.getWeapon;
		Vector3 enemyTipsPos = enemySword.getSwordTip.position;
		Vector3 enemySwordDir = -enemySword.transform.right;

		float enemyTargetAngle = enemy.weaponsOfficer.armBlockState.sideTargetAngle;
		
		aiCombat.MoveHandsToSidePos(-5f);

		if (enemyTargetAngle < 0f)
		{
			rotDir = -rotDir;
			aiCombat.MoveHandsToSidePos(5f);
		}

		if (Mathf.Abs(enemyTargetAngle) > 80f)
		{
			input.rArmVert = -1f;
		}
		else
		{
			float enemyPos = enemy.weaponsOfficer.armBlockState.rArmPos.y;
			float underEnemyPos =  enemyPos - posUnderEnemyArmPos;
			float asd = Mathf.Lerp(enemyPos, underEnemyPos, Mathf.Abs(enemyTargetAngle) / 80f);
			//print(arms.armBlockState.rArmPos.y + " " + asd);
			if (arms.armBlockState.rArmPos.y > asd)
			{
				input.rArmVert = -1f;
			}
			else
			{
				input.rArmVert = 1f;
			}
			//aiCombat.MoveHandsToPos(aiCombat.localHandBasePos);
		}

		float dirDot = Vector3.Dot(-arms.getWeapon.transform.right, enemySwordDir);
		//print(dirDot);
		if (dirDot > 0.4f)
		{
			input.rArmRot = rotDir;
		}
		else if (dirDot < 0.3f)
		{
			input.rArmRot = -rotDir;
		}
	}

	protected override void Update()
	{
		base.Update();
		input.rArmRot = 0f;
		input.rArmHorz = 0f;
		input.rArmVert = 0f;

		float rotDir = 1f;

		if (aiCombat.combatState == AI_Combat.CombatState.Defend)
		{
			CrossEnemySwordDir(rotDir);
		}
	}
}