using UnityEngine;

public class InputManager : MonoBehaviour
{
	public Axis[] axes = new Axis[2];
	public Button[] buttons = new Button[2];

	#region Buttons
	public void SetButton(string keyName, KeyCode key)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i].buttonName == keyName)
			{
				buttons[i].buttonName = keyName;
				buttons[i].key = key;
				break;
			}
		}
	}

	public bool GetButton(string keyName)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (Input.GetKey(buttons[i].key))
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtondDown(string keyName)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (Input.GetKeyDown(buttons[i].key))
			{
				return true;
			}
		}
		return false;
	}

	public Button GetActualButton(string buttonName)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i].buttonName == buttonName)
			{
				return buttons[i];
			}
		}

		return null;
	}
	#endregion

	#region Axes
	public void SetAxis(string axisName, KeyCode negativeKey, KeyCode positiveKey)
	{
		for (int i = 0; i < axes.Length; i++)
		{
			if (axes[i].axisName == axisName)
			{
				axes[i].positiveKey = positiveKey;
				axes[i].negativeKey = negativeKey;
				break;
			}
		}
	}

	public float GetAxis(string axisName)
	{
		for (int i = 0; i < axes.Length; i++)
		{
			if (axes[i].axisName == axisName)
			{
				if (Input.GetKey(axes[i].positiveKey))
				{
					axes[i].value += axes[i].sensitity * Time.deltaTime;
				}
				else if (Input.GetKey(axes[i].negativeKey))
				{
					axes[i].value -= axes[i].sensitity * Time.deltaTime;
				}
				else
				{
					if (axes[i].value > 0)
					{
						axes[i].value -= axes[i].gravity * Time.deltaTime;
						if (axes[i].value < axes[i].gravity * Time.deltaTime)
						{
							axes[i].value = 0f;
						}
					}
					else if (axes[i].value < 0)
					{
						axes[i].value += axes[i].gravity * Time.deltaTime;
						if (axes[i].value > -axes[i].gravity * Time.deltaTime)
						{
							axes[i].value = 0f;
						}
					}
				}

				axes[i].value = Mathf.Clamp(axes[i].value, -1f, 1f);
				return axes[i].value;
			}
		}
		return 0;
	}

	public float GetAxisRaw(string axisName)
	{
		for (int i = 0; i < axes.Length; i++)
		{
			if (axes[i].axisName == axisName)
			{
				if (Input.GetKey(axes[i].positiveKey))
				{
					return 1;
				}
				else if (Input.GetKey(axes[i].negativeKey))
				{
					return -1;
				}

				break;
			}
		}

		return 0;
	}

	public Axis GetActualAxis(string axisName)
	{
		for (int i = 0; i < axes.Length; i++)
		{
			if (axes[i].axisName == axisName)
			{
				return axes[i];
			}
		}

		return null;
	}
	#endregion
}

[System.Serializable]
public class Button
{
	public string buttonName;
	public KeyCode key;
}

[System.Serializable]
public class Axis
{
	public string axisName;
	public float gravity = 3f;
	public float sensitity = 3f;
	public KeyCode positiveKey;
	public KeyCode negativeKey;
	[HideInInspector]public float value;
}