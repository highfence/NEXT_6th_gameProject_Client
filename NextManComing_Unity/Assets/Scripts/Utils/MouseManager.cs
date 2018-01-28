using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	public bool IsEnterPressed = false;

	private ProtoPlayerControll player = null;

	[SerializeField]
	public int screenWidth = 0;

	public float boundary = Screen.width * 0.1f;

	public float mouseX = 0;
	public float mouseY = 0;

	private void Start()
	{
		player = GameObject.Find("Player").GetComponent<ProtoPlayerControll>();

		screenWidth = Screen.width;
	}

	private void Update()
	{
		AccordMouseToUI();

		if (IsEnterPressed == false)
		{
		}
	}

	// TODO :: 이렇게 하고 보니 엔터키의 상태를 일정하게
	// 보관하는 어떤 지표를 만드는 것이 낫겠다 싶음.
	private void AccordMouseToUI()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (IsEnterPressed)
			{
				Cursor.visible = false;
				IsEnterPressed = false;
			}
			else
			{
				IsEnterPressed = true;
				Cursor.visible = true;
			}
		}
	}
}
