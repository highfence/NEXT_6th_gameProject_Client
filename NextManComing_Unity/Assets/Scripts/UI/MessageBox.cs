using UnityEngine;
using UnityEngine.UI;

// https://www.youtube.com/watch?v=EeomgEjL6bc 를 보고 만든 간단한 팝업 창 클래스

public class MessageBox : MonoBehaviour
{
	public GameObject Window;
	public GameObject OkayButton;
	public Text MessageField;

	// 시작시에 자기 자신을 우선 숨김.
	public void Start()
	{
		this.Hide();
	}


	// 기본적인 메시지 박스를 보여주는 메서드.
	public void Show(string message)
	{
		MessageField.text = message;
		Window.SetActive(true);
	}


	// 버튼이 없는 메시지 박스를 보여주는 메서드.
	public void ShowWithNoButton(string message)
	{
		Show(message);
		OkayButton?.SetActive(false);		
	}


	// 메시지 박스를 숨기는 메서드.
	public void Hide()
	{
		Window.SetActive(false);
	}
}
