using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
	public GameObject Window;
	public Text MessageField;

	public void Show(string message)
	{
		MessageField.text = message;
		Window.SetActive(true);
	}

	public void Hide()
	{
		Window.SetActive(false);
	}
}
