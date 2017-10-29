using UnityEngine;
using Packet;

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

	private void InvokePacketEvents(Packet.Packet receivedPacket)
	{

	}

}