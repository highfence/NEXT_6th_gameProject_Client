using System;
using UnityEngine;
using PacketInfo;


 // NetworkManager 이벤트들을 정리하는 파셜 클래스.
 // 해당 패킷을 전달 받았을 때 실행시키고 싶은 이벤트를 걸어주면 된다.
 
internal partial class NetworkManager : MonoBehaviour
{
	public event Action<ServerConnectRes> OnServerConnectRes = delegate { };
}
