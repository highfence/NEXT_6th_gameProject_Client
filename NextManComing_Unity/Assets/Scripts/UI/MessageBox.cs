using UnityEngine;
using UnityEngine.UI;

// https://www.youtube.com/watch?v=EeomgEjL6bc 를 보고 만든 간단한 팝업 창 클래스

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
