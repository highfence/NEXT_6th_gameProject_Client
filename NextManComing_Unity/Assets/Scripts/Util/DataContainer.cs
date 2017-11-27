﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클라이언트 로직에 필요한 데이터들을 담아두는 클래스.
// 씬 전환시에 관련 씬 매니저가 할당 해제되어도 기록되어야 할 필요가 있는 정보를 기록한다.
public class DataContainer : MonoBehaviour
{
	#region SINGLETON

	private static DataContainer instance = null;

	private void Awake()
	{
		Initialize();
		DontDestroyOnLoad(this.gameObject);
	}

	public static DataContainer GetInstance()
	{
		if (instance == null)
		{
			instance = Instantiate(Resources.Load("Prefabs/DataContainer") as GameObject).GetComponent<DataContainer>();
		}

		return instance;
	}

	#endregion

	// 초기화 메소드.
	// 보유한 자료중에 초기화가 필요한 자료가 있다면 여기서 처리.
	private void Initialize()
	{
		LoadConfigs();
	}

	// 설정 파일 로드 메소드.
	private void LoadConfigs()
	{
		var loginServerConfigText = Resources.Load<TextAsset>("Data/LoginServerConfig").text;

		if (loginServerConfigText != null)
		{
			config = LoginServerConfig.CreateFromText(loginServerConfigText);
		}
	}

	private LoginServerConfig config;
}

public struct LoginServerConfig
{
	[SerializeField]
	public string LoginServerAddr;

	[SerializeField]
	public string Port;

	public static LoginServerConfig CreateFromText(string text)
	{
		LoginServerConfig instance;
		try
		{
#if DEBUG
			instance = new LoginServerConfig()
			{
				LoginServerAddr = "localhost",
				Port = "19000"
			};
#else
            instance = JsonUtility.FromJson<LoginServerConfig>(text);
#endif
		}
		catch (Exception e)
		{
			Debug.LogErrorFormat("[Config] Cannot parse Config from source : {0}, Error : {1}", text, e.Message);
			throw;
		}

		return instance;
	}

	public string GetUri()
	{
		var connectionString = "http://" + LoginServerAddr + ":" + Port + "/";
		return connectionString;
	}
}

