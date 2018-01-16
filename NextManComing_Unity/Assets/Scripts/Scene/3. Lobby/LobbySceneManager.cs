using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
	private MouseManager mouseManager = null;

	public void Start()
	{
		var mouseMgrPrefab = Instantiate(Resources.Load("Prefabs/MouseManager")) as GameObject;
		mouseManager = mouseMgrPrefab.GetComponent<MouseManager>();
		mouseManager.transform.SetParent(this.transform);
	}
}
