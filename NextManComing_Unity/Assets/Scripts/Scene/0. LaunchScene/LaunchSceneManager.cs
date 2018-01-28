using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 시작전 처리가 필요한 작업을 미리 진행하는 클래스.
/// 스크린, 마우스 설정. 리소스 로드. 모든 싱글톤 클래스의 인스턴스화를 담당한다.
/// 추가적으로 로딩 씬을 재생한다.
/// </summary>
public class LaunchSceneManager : MonoBehaviour
{
	/// <summary>
	/// 모든 초기화 작업을 시작한다.
	/// TODO :: 클라이언트 설정을 Config로 읽어와야 한다.
	/// </summary>
	private void Start()
	{
		ScreenInitialize(1280, 800, false);

		MouseInitialize();

		StartCoroutine("OnClassLoad");

		DataStorage.GetInstance();

		NetworkManager.GetInstance();
	}

	/// <summary>
	/// 마우스 설정 초기화 메서드.
	/// </summary>
	private static void MouseInitialize()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	/// <summary>
	/// 스크린 설정 초기화 메서드.
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="isFullScreen"></param>
	private static void ScreenInitialize(int width, int height, bool isFullScreen)
	{
		Screen.SetResolution(width, height, isFullScreen);
	}

	/// <summary>
	/// 초기화가 진행되는 동안 시간을 벌며 시작 로딩씬을 진행해주는 메서드.
	/// </summary>
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
