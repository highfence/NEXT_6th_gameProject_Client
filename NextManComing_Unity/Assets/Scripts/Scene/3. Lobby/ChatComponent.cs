using UnityEngine;
using TcpPacket;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 로비씬 플레이어의 채팅을 가능하게 하는 컴포넌트.
/// </summary>
public class ChatComponent : MonoBehaviour
{
	// 메시지의 백그라운드 이미지를 담당하는 멤버.
	public GameObject MessageBox;

	// 메시지 텍스트를 동적할당이 아니라 에디터에서 효율적으로 관리하도록 하기 위한 멤버.
	public GameObject Message;

	// 실질적으로 메시지의 텍스트를 담고있는 멤버.
	public TextMesh MsgText;

	// 메시지 박스가 사용 가능한 상황인지 알려주는 멤버.
	public bool IsValiable = false;


	private struct SendedMessage
	{
		public string Message;
		public long SendedTime;

		public SendedMessage(LobbyChatReq requestPacket)
		{
			Message = requestPacket.Message;
			SendedTime = requestPacket.Time;
		}
	}

	private Queue<SendedMessage> sendedMsgQueue = null;
	private NetworkManager network  = null;
	private DataStorage dataStorage = null;

	private void Start()
	{
		// 다른 무엇보다 자기 자신이 보이지 않도록 우선 설정.
		this.gameObject.SetActive(false);

		dataStorage = DataStorage.GetInstance();
		sendedMsgQueue = new Queue<SendedMessage>();

		if (NetworkInitialize() != false || UIInitialize() != false)
		{
			IsValiable = true;	
		}
	}


	/// <summary>
	/// 사용할 UI 요소들을 초기화해주는 메서드.
	/// </summary>
	/// <returns></returns>
	private bool UIInitialize()
	{
		if (MessageBox == null || Message == null)
		{
			return false;
		}

		// GetComponent의 비싼 비용을 줄이기 위해 미리 받아놓는다.
		MsgText = Message.GetComponent<TextMesh>();

		return true;
	}

	
	/// <summary>
	/// 네트워크 관리자를 설정해주고 사용할 네트워크 이벤트들을 등록해주는 메서드.
	/// </summary>
	/// <returns></returns>
	private bool NetworkInitialize()
	{
		network = NetworkManager.GetInstance();
		if (network == null)
		{
			return false;
		}

		network.OnLobbyChatRes += OnLobbyChatRes;	

		return true;
	}


	/// <summary>
	/// 메시지를 설정하고 이를 UI에 띄워준다. 내부에서는 우선 메시지를 받아 패킷을 보내주고, 이에 해당하는 응답이 올 경우에 메시지를 띄운다. 
	/// </summary>
	/// <param name="msg"></param>
	/// <returns></returns>
	public bool SetMessage(string msg)
	{
		if (IsValiable == false)
		{
			return false;
		}

		// WARN :: 여기 만든 패킷은 서버와 협의되지 않음.
		var message = new LobbyChatReq()
		{
			Id = dataStorage.Id,
			Token = dataStorage.Token,
			Message = msg,
			Time = DateTime.Now.Ticks
		};

		network.SendPacket(message, PacketId.LobbyChatReq);

		// 메시지를 보냈다면 이에 해당하는 답변이 올때까지 저장해둔다.
		sendedMsgQueue.Enqueue(new SendedMessage(message));

		return true;
	}


	/// <summary>
	/// 내가 보낸 로비 채팅의 결과가 도착했을 때 이를 실제로 화면에 그려주는 메서드.
	/// </summary>
	/// <param name="packet"></param>
	private void OnLobbyChatRes(LobbyChatRes packet)
	{
		// TODO :: 이 지점에서 어떤 처리를 해줄 수 있을까? 서버에서 채팅이 안된다고 나왔는데?
		if (packet.Result != 0)
			return;

		string showMsg = "";

		foreach (var sendedMsg in sendedMsgQueue)
		{
			// 패킷이 ReceivedTime보다 전에 보낸 친구들까지만 보여준다.
			if (sendedMsg.SendedTime <= packet.ReceivedTime)
			{
				// 이전에 추가했던 메시지가 있다면 엔터를 추가해준다.
				if (showMsg != "")
				{
					showMsg += System.Environment.NewLine;
				}

				showMsg += sendedMsg.Message;
			}
			else
			{
				break;
			}
		}

		MsgText.text = showMsg;
		
	}


	IEnumerator SetChatTimerOn(Int64 msgTime, float duration)
	{
		yield return new WaitForSeconds(duration);


	}
}
