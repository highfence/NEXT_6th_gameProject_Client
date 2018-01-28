using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatInputField : MonoBehaviour
{
	public InputField InputField = null;

	public Action<string> OnChatWriteEnded = delegate { };

	public void Start()
	{
		// 우선 인풋 필드를 보이지 않게 바꿔준다.
		InputField?.gameObject.SetActive(false);

		RegistInputEvents();
	}

	private void RegistInputEvents()
	{
		var gameInputLayer = FindObjectOfType<GameInputLayer>();

		if (gameInputLayer == null)
		{
			Debug.LogAssertion("There is no GameInputLayer in this game.");
			return;
		}

		gameInputLayer.OnEnterKeyPressed += AccordStatusToEnter;
	}

	private void AccordStatusToEnter(bool isEnterKeyPressed)
	{
		if (isEnterKeyPressed)
		{
			InputField.gameObject.SetActive(true);
			InputField.ActivateInputField();
		}
		else
		{
			if (InputField == null) return;

			var sendMsg = InputField.text;
			InputField.text = "";
			InputField.gameObject.SetActive(false);
		}
	}
}
