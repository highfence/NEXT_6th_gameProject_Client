using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로비씬 플레이어의 채팅을 가능하게 하는 컴포넌트.
/// </summary>
public class ChatService : MonoBehaviour
{
	// 메시지의 백그라운드 이미지를 담당하는 멤버.
	[SerializeField]
	public GameObject MessageBox;

	// 메시지 텍스트를 동적할당이 아니라 에디터에서 효율적으로 관리하도록 하기 위한 멤버.
	[SerializeField]
	public GameObject Message;

	// 실질적으로 메시지의 텍스트를 담고있는 멤버.
	public TextMesh MsgText;

	// 메시지 박스가 사용 가능한 상황인지 알려주는 멤버.
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
		if (MessageBox == null || Message == null)
		{
			return false;
		}

		// GetComponent의 비싼 비용을 줄이기 위해 미리 받아놓는다.
		MsgText = Message.GetComponent<TextMesh>();

		MessageBox.SetActive(false);

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

		// WARN :: 여기 만든 패킷은 서버와 협의되지 않음.

		return true;
	}
}
