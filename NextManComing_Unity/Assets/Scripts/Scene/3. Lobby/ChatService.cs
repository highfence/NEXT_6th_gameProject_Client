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

	[SerializeField]
	public bool IsValiable = false;

	private NetworkManager network = null;


	private void Start()
	{
		network = NetworkManager.GetInstance();

		if (network == null || UIInitialize() != false)
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
		

		return true;
	}

	/// <summary>
	/// 메시지를 설정하고 이를 UI에 띄워준다. 
	/// 내부에서는 우선 메시지를 받아 패킷을 보내주고, 이에 해당하는 응답이 올 경우에
	/// 메시지를 띄운다. (따라서 다소의 레이턴시가 발생할 수 있다.)
	/// </summary>
	/// <param name="msg"></param>
	/// <returns></returns>
	public bool SetMessage(string msg)
	{
		if (IsValiable == false)
		{
			return false;
		}

		return true;
	}

}
