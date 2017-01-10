//using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabMister))]
public class PrefabMisterEditor : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		PrefabMister prefabMister = (PrefabMister)target;

		if (prefabMister.prefabs.Length > 0 &&
			GUILayout.Button("Update prefabs"))
		{
			prefabMister.UpdatePrefabs();
		}

		if (GUILayout.Button("Update all scene prefabs"))
		{
			prefabMister.UpdateAllScenePrefabs();
		}
	}
}