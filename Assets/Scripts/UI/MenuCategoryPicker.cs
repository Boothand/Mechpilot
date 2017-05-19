using System.Collections;
using UnityEngine;
using TMPro;

public class MenuCategoryPicker : MonoBehaviour
{
	[SerializeField] TMP_Text[] textElements;
	[SerializeField] GameObject[] categories;
	int selectedCategory;

	private void Start()
	{
		Highlight(selectedCategory);
	}

	public void Highlight(int index)
	{
		textElements[index].GetComponent<Animator>().SetTrigger("Highlighted");
	}

	public void Deselect(int index)
	{
		if (index != selectedCategory)
		{
			textElements[index].GetComponent<Animator>().SetTrigger("Deselected");
			categories[index].SetActive(false);
		}
	}

	public void SelectCategory(int index)
	{
		selectedCategory = index;

		for (int i = 0; i < textElements.Length; i++)
		{
			if (i != index)
			{
				Deselect(i);
			}
		}

		Highlight(index);
		categories[index].SetActive(true);
	}
}