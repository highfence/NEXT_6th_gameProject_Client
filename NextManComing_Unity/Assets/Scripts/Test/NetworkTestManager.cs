using System;
using UnityEngine;

public class NetworkTestManager : MonoBehaviour
{
	private NetworkManager network;

	[SerializeField]
	private string id = "";
	[SerializeField]
	private long token = 0;

	private void Awake()
	{
		network = NetworkManager.GetInstance();
	}

	public void OnSendButtonClicked()
	{
		var serverConnectReq = new ServerConnectReq()
		{
			Id = id,
			Token = token
		};

		network.SendPacket(serverConnectReq, PacketId.ServerConnectReq);
	}
}
