//using System.Collections;
using UnityEngine;
using UnityEditor;
using RootMotion.FinalIK;

[CustomEditor(typeof(IKGroup))]
public class IKPlugEditor : Editor
{
	FullBodyBipedIK ik;
	LookAtIK lookIK;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		IKGroup ikGroup = (IKGroup)target;

		ik = ikGroup.transform.root.GetComponentInChildren<FullBodyBipedIK>();
		lookIK = ikGroup.transform.root.GetComponentInChildren<LookAtIK>();

		if (GUILayout.Button("Hook up all"))
		{
			foreach (Transform child in ikGroup.transform)
			{
				if (child.name == "Elbow R")
					ik.solver.rightArmChain.bendConstraint.bendGoal = child;
				else if (child.name == "Elbow L")
					ik.solver.leftArmChain.bendConstraint.bendGoal = child;
				else if (child.name == "Shoulder R")
					ik.solver.GetEffector(FullBodyBipedEffector.RightShoulder).target = child;
				else if (child.name == "Shoulder L")
					ik.solver.GetEffector(FullBodyBipedEffector.LeftShoulder).target = child;
				else if (child.name == "Body Target")
					lookIK.solver.target = child;
				else
					ik.solver.rightHandEffector.target = child;
			}
		}

		if (GUILayout.Button("Hook up all (child)"))
		{
			foreach (Transform child in ikGroup.transform)
			{
				Transform transformToUse = child;

				if (child.childCount > 0)
					transformToUse = child.GetChild(0);

				if (child.name == "Elbow R")
					ik.solver.rightArmChain.bendConstraint.bendGoal = transformToUse;
				else if (child.name == "Elbow L")
					ik.solver.leftArmChain.bendConstraint.bendGoal = transformToUse;
				else if (child.name == "Shoulder R")
					ik.solver.GetEffector(FullBodyBipedEffector.RightShoulder).target = transformToUse;
				else if (child.name == "Shoulder L")
					ik.solver.GetEffector(FullBodyBipedEffector.LeftShoulder).target = transformToUse;
				else if (child.name == "Body Target")
					lookIK.solver.target = transformToUse;
				else
					ik.solver.rightHandEffector.target = transformToUse;
			}
		}

		if (GUILayout.Button("Reset"))
		{
			ik.solver.rightHandEffector.target = ikGroup.transform.root.Find("Walk_Test").FindChild("rHandIKTarget");
			ik.solver.leftHandEffector.target = ikGroup.transform.root.Find("Walk_Test").FindChild("lHandIKTarget");
			ik.solver.rightArmChain.bendConstraint.bendGoal = ikGroup.transform.root.Find("Walk_Test").FindChild("rElbowIKTarget");
			ik.solver.leftArmChain.bendConstraint.bendGoal = ikGroup.transform.root.Find("Walk_Test").FindChild("lElbowIKTarget");
			ik.solver.GetEffector(FullBodyBipedEffector.RightShoulder).target = ikGroup.transform.root.Find("Walk_Test").FindChild("rShoulderIKTarget");
			ik.solver.GetEffector(FullBodyBipedEffector.LeftShoulder).target = ikGroup.transform.root.Find("Walk_Test").FindChild("lShoulderIKTarget");
		}
	}
}