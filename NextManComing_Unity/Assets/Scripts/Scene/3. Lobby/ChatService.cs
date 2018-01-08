using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로비씬 플레이어의 채팅을 가능하게 하는 컴포넌트.
/// </summary>
public class ChatService : MonoBehaviour
{
	[SerializeField]
	public GameObject MessageBox;

	[SerializeField]
	public GameObject Message;

	private NetworkManager network;

	private void Start()
	{
		network = NetworkManager.GetInstance();
	}


}
