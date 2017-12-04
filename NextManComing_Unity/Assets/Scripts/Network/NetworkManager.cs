using UnityEngine;
using MessagePack;
using TcpPacket;
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

	#endregion

	public void Initialize()
	{
		// HttpNetwork 생성
		HttpHandler = Instantiate(Resources.Load("Prefabs/HttpHandler") as GameObject).GetComponent<HttpNetwork>();
	}

	public void TcpConnect(string serverIp, int serverPort)
	{
		// TcpNetwork 생성.
		TcpHandler = new TcpNetwork(serverIp, serverPort);	
		TcpHandler.ConnectToServer();
	}

	public void TcpClose()
	{
		TcpHandler?.CloseNetwork();
	}

	// 어플리케이션이 종료될 때 소켓을 닫아주는 메소드.
	private void OnApplicationQuit()
	{
		// TODO :: Close Session Packet을 보내준다.

		TcpHandler?.CloseNetwork();
	}

	private void Update()
	{
		// 아직 Tcp핸들러가 초기화되지 않았거나, 메시지가 없다면 바로 리턴.
		if (TcpHandler == null || TcpHandler.IsMessageExist() == false) return;

		var receivedPacket = TcpHandler.GetPacketFromQueue();

		InvokePacketEvents(receivedPacket);
	}

	private void InvokePacketEvents(Packet receivedPacket)
	{
		switch ((PacketId)receivedPacket.PacketId)
		{
			case PacketId.ServerListRes :
				OnServerListRes.Invoke(MessagePackSerializer.Deserialize<ServerListRes>(receivedPacket.Data));
				break;
			case PacketId.ServerConnectRes :
				OnServerConnectRes.Invoke(MessagePackSerializer.Deserialize<ServerConnectRes>(receivedPacket.Data));
				break;
		}
	}

	// 컴포넌트 HttpNetwork의 PostRequest 래핑 메소드.
	public void HttpPost<REQUEST_T, RESULT_T>(string url, REQUEST_T bodyPacket, Func<RESULT_T, bool> onResultArrivedCallback)
	{
		StartCoroutine(HttpHandler.PostRequest<REQUEST_T, RESULT_T>(url, bodyPacket, onResultArrivedCallback));
	}

	// 컴포넌트 TcpNetwork의 Send를 호출해주는 래핑 메소드.
	public void SendPacket<T>(T data, PacketId packetId)
	{
		TcpHandler.SendPacket(data, packetId);
	}
}