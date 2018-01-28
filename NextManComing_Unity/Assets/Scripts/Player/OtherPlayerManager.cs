using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 다른 플레이어의 정보를 나타내는 클래스.
// TODO :: 어떤 방식으로 서버와 소통할 것인지 필요함.
public class PlayerInfo
{

}

// 클라이언트에서 표시될 다른 플레이어들을 관리하는 클래스.
public class OtherPlayerManager : MonoBehaviour
{
	private GameObject[] playerPool = null;
	private readonly int poolSize = 10;

	private void Start()
	{
		playerPool = new GameObject[poolSize];

		// TODO :: 프리팹 지정해줘야함.
		var playerPrefab = Resources.Load("Prefabs/TestNpc");
		
		for (var i = 0; i < poolSize; ++i)
		{
			playerPool[i] = Instantiate(playerPrefab) as GameObject;
			playerPool[i].name = "Player_" + i;
			playerPool[i].SetActive(false);
		}
	}
}	