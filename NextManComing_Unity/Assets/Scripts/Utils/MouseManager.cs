using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	public bool IsEnterPressed = false;

	public GameObject Player = null;

	[SerializeField]
	public int screenWidth = Screen.width;
	[SerializeField]
	public readonly int boundary = Convert.ToInt32(Screen.width * 0.1f);

	private void Start()
	{
		Player = GameObject.Find("Player");
	}

	private void Update()
	{
		AccordMouseToUI();

		RotateOnBoundary();
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


	// 마우스가 화면의 가장자리에 있다고 판단되었을 때
	// 계속 화면을 돌려주는 함수.
	private void RotateOnBoundary()
	{
		// 오른쪽으로 돌기.
		if (Input.mousePosition.x > screenWidth - boundary)
		{
		}
		// 왼쪽으로 돌기.
		if (Input.mousePosition.x < 0 + boundary)
		{
		}
	}
}
