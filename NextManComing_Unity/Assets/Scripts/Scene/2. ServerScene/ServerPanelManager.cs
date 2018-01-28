using System.Collections;
using System.Collections.Generic;
using TcpPacket;
using UnityEngine;
using UnityEngine.Assertions;

public class ServerPanelManager : MonoBehaviour
{
	[SerializeField]
	GameObject panelPrefab;

	[SerializeField]
	UISystem uiSystem;


	GameObject[] panelPool = null;
	readonly int poolSize = 10;


	private void Start()
	{
		Assert.IsNotNull(panelPrefab);

		panelPool = new GameObject[poolSize];

		for (var i = 0; i < poolSize; ++i)
		{
			panelPool[i] = Instantiate(panelPrefab) as GameObject;
			panelPool[i].name = "Panel_" + i;
			panelPool[i].SetActive(false);

			uiSystem.AttachUI(panelPool[i]);
		}
	}


	public void SetPanels(ServerListRes result)
	{
		var firstPosition = new Vector3(Screen.width * 0.15f, Screen.height * 0.15f, 0f);

		for (var i = 0; i < result.ServerCount; ++i)
		{
			panelPool[i].transform.position = firstPosition;
			panelPool[i].SetActive(true);

			var panel = panelPool[i].GetComponent<ServerPanel>();
			panel.SetInfo("Server " + i.ToString(), result.ServerCountList[i], result.ServerList[i]);
			
			firstPosition.y += 60f;
		}
	}
}
