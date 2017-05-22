//using System.Collections;
using UnityEngine;

public class PrefabMister : MonoBehaviour
{
	[SerializeField] public GameObject[] prefabs = new GameObject[1];
	[SerializeField] bool ignore;
	
	void Awake()
	{
		UpdatePrefabs();
	}

	public void UpdatePrefabs()
	{
		if (ignore)
			return;

		for (int i = 0; i < prefabs.Length; i++)
		{
			Transform currentObj = transform.Find(prefabs[i].name);
			if (currentObj)
			{
				Debug.Log("Replaced " + currentObj.name + " with prefab.", this as Object);
				Vector3 oldPos = currentObj.localPosition;
				Quaternion oldRot = currentObj.localRotation;
				Vector3 oldScale = currentObj.localScale;

				DestroyImmediate(currentObj.gameObject);

				GameObject newObj = Instantiate(prefabs[i], transform) as GameObject;
				newObj.transform.localPosition = oldPos;
				newObj.transform.localRotation = oldRot;
				newObj.transform.localScale = oldScale;
				newObj.name = prefabs[i].name;
			}
		}
	}

	public void UpdateAllScenePrefabs()
	{
		PrefabMister[] misters = GameObject.FindObjectsOfType<PrefabMister>();

		for (int i = 0; i < misters.Length; i++)
		{
			misters[i].UpdatePrefabs();
		}
	}
}