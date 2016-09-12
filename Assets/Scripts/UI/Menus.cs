using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menus : MonoBehaviour
{
	static Stack<GameObject> menuStack = new Stack<GameObject>();

	void Start ()
	{

	}

	public static void InitializeStack(GameObject baseMenu)
	{
		if (menuStack.Count > 0)
		{
			Debug.Log("Already basemenu in place: " + menuStack.Peek().transform.name);
			return;
		}

		menuStack.Push(baseMenu);
	}

	public void DisplayMenu(GameObject menuObj)
	{
		StartCoroutine(StackMenuRoutine(menuObj, 0.5f));
	}

	public void GoBack()
	{
		StartCoroutine(PopMenuRoutine());
	}

	IEnumerator StackMenuRoutine(GameObject nextMenu, float transitionTime)
	{
		GameObject currentMenu = null;
		if (menuStack.Count > 0)
		{
			currentMenu = menuStack.Peek();
			currentMenu.SetActive(false);
		}

		menuStack.Push(nextMenu);

		nextMenu.SetActive(true);
		nextMenu.transform.localPosition = Vector3.zero;

		yield return new WaitForEndOfFrame();
	}

	IEnumerator PopMenuRoutine()
	{
		if (menuStack.Count < 1)
		{
			Debug.Log("Already at lowest menu level.");
			yield break;
		}

		GameObject currentMenu = menuStack.Pop();
		GameObject nextMenu = menuStack.Peek();

		currentMenu.SetActive(false);
		Debug.Log("Hiding " + currentMenu.name);
		print("Showing " + nextMenu.name);
		nextMenu.SetActive(true);
		nextMenu.transform.localPosition = Vector3.zero;

		yield return new WaitForEndOfFrame();
	}

	public void QuitToDesktop()
	{
		Application.Quit();
	}
	
	void Update ()
	{
		
	}
}