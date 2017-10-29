using MessagePack;
using UnityEngine;
using PacketInfo;
using System;

internal partial class NetworkManager : MonoBehaviour
{
	public HttpNetwork HttpHandler { get; private set; }
	public TcpNetwork TcpHandler { get; private set; }

	#region SINGLETON

	private static NetworkManager _instance = null;

	private void Awake()
	{
		Initialize();
		DontDestroyOnLoad(this.gameObject);
	}

	public static NetworkManager GetInstance()
	{
		if (_instance == null)
		{
			_instance = Instantiate(Resources.Load("Prefabs/NetworkManager") as GameObject).GetComponent<NetworkManager>();
		}
		return _instance;
	}

	public void Initialize()
	{
		// TcpNetwork 생성.
		TcpHandler = new TcpNetwork("10.73.43.213");
		TcpHandler.ConnectToServer();

		// HttpNetwork 생성
		HttpHandler = Instantiate(Resources.Load("Prefabs/HttpHandler") as GameObject).GetComponent<HttpNetwork>();
	}

	#endregion

	// 어플리케이션이 종료될 때 소켓을 닫아주는 메소드.
	private void OnApplicationQuit()
	{
		// TODO :: Close Session Packet을 보내준다.

		TcpHandler.CloseNetwork();
	}

	private void Update()
	{
		if (TcpHandler.IsMessageExist())
		{
			var receivedPacket = TcpHandler.GetPacketFromQueue();

			InvokePacketEvents(receivedPacket);
		}
	}

	private void InvokePacketEvents(Packet receivedPacket)
	{
		switch ((PacketId)receivedPacket.PacketId)
		{
			case PacketId.ServerConnectRes :
				OnServerConnectRes.Invoke(MessagePackSerializer.Deserialize<ServerConnectRes>(receivedPacket.Data));
				break;
		}
	}

	// 컴포넌트 HttpNetwork의 PostRequest 래핑 메소드.
	public void HttpPost<T>(string url, string bodyJson, Func<T, bool> onSuccess)
	{
		StartCoroutine(HttpHandler.PostRequest<T>(url, bodyJson, onSuccess));
	}

	// 컴포넌트 TcpNetwork의 Send를 호출해주는 래핑 메소드.
	public void SendPacket<T>(T data, PacketId packetId)
	{
		TcpHandler.SendPacket(data, packetId);
	}
}