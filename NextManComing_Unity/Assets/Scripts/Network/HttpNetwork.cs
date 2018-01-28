using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MessagePack;

public class HttpNetwork : MonoBehaviour
{
	// Http Post를 보내주는 메소드
	// @ Param 1 : 접속하려는 url
	// @ Param 2 : 보내려는 Json Serialized 구조체
	// @ Param 3 : Post가 성공했을 경우 반환 값이 인자로 들어가는 콜백 함수.
	public IEnumerator PostRequest<REQUEST_T, RESULT_T>(string url, REQUEST_T bodyPacket, Func<RESULT_T, bool> onResultArrivedCallback)
	{
		var request = new UnityWebRequest(url, "POST");
		var bodyJsonString = JsonUtility.ToJson(bodyPacket); 
		var bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);

		request.uploadHandler = new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		// IO가 끝날때까지 사용권 반환.
		yield return request.Send();

		if (request.isNetworkError)
		{
			Debug.LogError("Http Post Failed");
		}
		else
		{
			// 성공했을 경우.
			switch (request.responseCode)
			{
				case 200 :
					// 받은 정보를 처리하도록 넘겨준다.
					onResultArrivedCallback(JsonUtility.FromJson<RESULT_T>(request.downloadHandler.text));
					break;

				case 401 :
					// 다시 한 번 요청을 보내준다.
					Debug.Log("Http Post Error 401 : Unauthorized. Resubmitted Request");
					StartCoroutine(PostRequest(url, bodyJsonString, onResultArrivedCallback));
					break;

				default :
					Debug.Log("Request failed (status : " + request.responseCode + ")");
					break;
			}
		}
	}
}