using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
	private GameInputManager mouseManager = null;

	public void Start()
	{
		MouseManagerInitialize();
	}

	public void MouseManagerInitialize()
	{
		var mouseMgrPrefab = Instantiate(Resources.Load("Prefabs/MouseManager")) as GameObject;
		mouseManager = mouseMgrPrefab.GetComponent<GameInputManager>();
		mouseManager.transform.SetParent(this.transform);
	}
}
