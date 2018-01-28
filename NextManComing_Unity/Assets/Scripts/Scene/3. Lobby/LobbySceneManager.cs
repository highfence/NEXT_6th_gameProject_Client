using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
	private GameInputLayer gameInputLayer = null;

	public void Start()
	{
		GameInputLayerInitialize();
	}

	public void GameInputLayerInitialize()
	{
		gameInputLayer = FindObjectOfType<GameInputLayer>();
	}
}
