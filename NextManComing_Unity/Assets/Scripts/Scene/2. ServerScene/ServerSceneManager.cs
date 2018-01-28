using System;
using System.Collections;
using TcpPacket;
using UnityEngine;
using UnityEngine.Assertions;

public class ServerSceneManager : MonoBehaviour
{
	private DataStorage    dataStorage;
	private NetworkManager network;
	private UISystem       uiSystem;

	[SerializeField]
	private MessageBox msgBox;

	[SerializeField]
	private ServerPanelManager panelManager;


	private void Awake()
	{
		dataStorage = DataStorage.GetInstance();
		network     = NetworkManager.GetInstance();
		uiSystem    = FindObjectOfType<UISystem>();
	}


	private void Start()
	{
		RegistPacketEvents();
		UIInitalize();
		GetServerList();
	}


	// 패킷 관련 이벤트들을 구독시키는 메서드.
	private void RegistPacketEvents()
	{
		network.OnServerListRes += OnServerListArrived;
	}


	// UI 오브젝트들을 초기화 시켜주는 메서드.
	private void UIInitalize()
	{
		Assert.IsNotNull(uiSystem);

		uiSystem.AttachUI(msgBox.gameObject);
		msgBox.ShowWithNoButton("Loading...");
	}


	// Manage 서버에 접속하여 서버 리스트를 받아오는 메서드.
	private void GetServerList()
	{
		var serverListReq = new ServerListReq()
		{
			Id = dataStorage.Id,
			Token = dataStorage.Token
		};

		network.SendPacket(serverListReq, PacketId.ServerListReq);
	}


	#region PACKET LOGIC FUNCTIONS


	private void OnServerListArrived(ServerListRes result)
	{
		Assert.IsNotNull(panelManager);

		panelManager.SetPanels(result);

		msgBox.Hide();
	}


	#endregion
}
