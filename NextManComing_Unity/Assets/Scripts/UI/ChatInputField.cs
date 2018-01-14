using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatInputField : MonoBehaviour
{
	public InputField InputField = null;

	public bool IsEnterPressed = false;

	// 우선 인풋 필드를 보이지 않게 바꿔준다.
	public void Start()
	{
		InputField?.gameObject.SetActive(false);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{ 
			if (IsEnterPressed == false)
			{
				IsEnterPressed = true;
				InputField.gameObject.SetActive(true);
				InputField.ActivateInputField();
			}
			else
			{
				IsEnterPressed = false;

				if (InputField != null)
				{
					var sendMsg = InputField.text;
					InputField.text = "";
					InputField.gameObject.SetActive(false);
				}
			}
		}
	}
}
