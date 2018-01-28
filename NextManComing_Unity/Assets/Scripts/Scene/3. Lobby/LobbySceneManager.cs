using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
	private GameInputLayer mouseManager = null;

	public void Start()
	{
		MouseManagerInitialize();
	}

	public void MouseManagerInitialize()
	{
		var mouseMgrPrefab = Instantiate(Resources.Load("Prefabs/MouseManager")) as GameObject;
		mouseManager = mouseMgrPrefab.GetComponent<GameInputLayer>();
		mouseManager.transform.SetParent(this.transform);
	}
}
