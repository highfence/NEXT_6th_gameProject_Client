using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PacketInfo;

public class NetworkTestManager : MonoBehaviour
{
	private NetworkManager network;

	[SerializeField]
	private string id;
	[SerializeField]
	private Int64 token;

	private void Awake()
	{
		network = NetworkManager.GetInstance();
	}

	public void OnSendButtonClicked()
	{
		var ServerConnectReq = new ServerConnectReq()
		{
			Id = id,
			Token = token
		};

		network.SendPacket(ServerConnectReq, PacketId.ServerConnectReq);
	}
}
