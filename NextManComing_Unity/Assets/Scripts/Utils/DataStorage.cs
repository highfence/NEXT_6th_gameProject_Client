using HttpPacket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// 클라이언트 로직에 필요한 데이터들을 담아두는 클래스.
// 씬 전환시에 관련 씬 매니저가 할당 해제되어도 기록되어야 할 필요가 있는 정보를 기록한다.
public class DataStorage : MonoBehaviour
{
	#region SINGLETON

	private static DataStorage instance = null;

	private void Awake()
	{
		Initialize();
		DontDestroyOnLoad(this.gameObject);
	}

	public static DataStorage GetInstance()
	{
		if (instance == null)
		{
			var prefab = Instantiate(Resources.Load("Prefabs/DataStorage")) as GameObject;

			Assert.IsNotNull(prefab);

			instance = prefab.GetComponent<DataStorage>();

			Assert.IsNotNull(instance);
		}

		return instance;
	}


	// 초기화 메소드.
	// 보유한 자료중에 초기화가 필요한 자료가 있다면 여기서 처리.
	private void Initialize()
	{
		LoadConfigs();
	}

	#endregion

	// 설정 파일 로드 메소드.
	private void LoadConfigs()
	{
		var loginServerConfigText = Resources.Load<TextAsset>("Data/LoginServerConfig").text;

		if (loginServerConfigText != null)
		{
			Config = LoginServerConfig.CreateFromText(loginServerConfigText);

			Debug.Log($"Login Server Config Loaded. Addr({Config.LoginServerAddr}), Port({Config.Port})");
		}
	}

	public LoginServerConfig Config { get; private set; }

	public string Id { get; private set; }
	public string Pw { get; private set; }

	public long Token { get; private set; }
	public string ManageServerAddr { get; private set; }
	public int ManageServerPort { get; private set; }

	// 로그인 정보를 저장해주는 메서드.
	public void LoginInfoStore(string id, string pw)
	{
		Id = id;
		Pw = pw;
	}

	// 로그인 결과가 도착했을 때 그 결과는 저장해주는 메서드.
	public void LoginResultStore(LoginRes response)
	{
		Token = response.Token;
		ManageServerAddr = response.ManageServerAddr;
		ManageServerPort = response.ManageServerPort;
	}
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
				Port = "18000"
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

