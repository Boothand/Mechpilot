using UnityEngine;

public class MenuManager : MonoBehaviour
{
	[SerializeField] Menu firstMenu;

	[SerializeField] Menu[] menus;

	[SerializeField] Texture2D cursorImg;

	void Awake()
	{

	}

	void Start()
	{
		ResolutionPicker.InitiateResolution();
		HideAllMenus();

		firstMenu.gameObject.SetActive(true);

		Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);
	}

	public void HideAllMenus()
	{
		for (int i = 0; i < menus.Length; i++)
		{
			menus[i].gameObject.SetActive(false);
		}
	}

	public void ShowMenu(Menu menu)
	{
		HideAllMenus();
		menu.gameObject.SetActive(true);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	void Update()
	{
		
	}
}