using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 시작 전에 처리해야할 것들을 처리하는 클래스.

public class LaunchSceneManager : MonoBehaviour
{
	private void Start()
	{
		// TODO :: 클라이언트 Config 읽어오기.
		ScreenInitialize(1280, 800, false);

		MouseInitialize();

		StartCoroutine("OnClassLoad");

		DataStorage.GetInstance();

		NetworkManager.GetInstance();
	}

	private static void MouseInitialize()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	private static void ScreenInitialize(int width, int height, bool isFullScreen)
	{
		Screen.SetResolution(width, height, isFullScreen);
	}

	// 초기화가 진행되는 동안 시간을 벌며 시작 스프라이트를 띄워주는 메소드.
	IEnumerator OnClassLoad()
	{
		var loadingRenderer = GetComponent<SpriteRenderer>();

		var curColor = loadingRenderer.color;
		curColor.a = 0.0f;
		loadingRenderer.color = curColor;

		while (curColor.a < 1.0f)
		{
			curColor.a += 0.02f;
			loadingRenderer.color = curColor;

			yield return new WaitForSeconds(0.025f);
		}

		yield return new WaitForSeconds(0.5f);

		SceneManager.LoadScene("1. Login");
	}
}
