﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpNetwork : MonoBehaviour
{
	// Http Post를 보내주는 메소드
	// @ Param 1 : 접속하려는 url
	// @ Param 2 : 보내려는 Json Serialized 구조체
	// @ Param 3 : Post가 성공했을 경우 반환 값이 인자로 들어가는 콜백 함수.
	public IEnumerator PostRequest<T>(string url, string bodyJsonString, Func<T, bool> onSuccess)
	{
		var request    = new UnityWebRequest(url, "POST");
		byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);

		request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		// IO가 끝날때까지 사용권 반환.
		yield return request.Send();

		if (request.isNetworkError)
		{
			Debug.LogError("Http Post Failed");
		}
		// 성공했을 경우.
		else if (request.responseCode == 200)
		{
			// 받은 정보를 처리하도록 넘겨준다.
			onSuccess(JsonUtility.FromJson<T>(request.downloadHandler.text));
		}
		else if (request.responseCode == 401)
		{
			Debug.Log("Http Post Error 401 : Unauthorized. Resubmitted Request");
			StartCoroutine(PostRequest<T>(url, bodyJsonString, onSuccess));
		}
		else
		{
			Debug.Log("Request failed (status : " + request.responseCode + ")");
		}
	}
}

// TODO :: Login 서버와의 통신을 위해 필요한 구조체 정의.