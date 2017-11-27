using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
	private DataContainer dataContainer;
	private NetworkManager network;

	public void Awake()
	{
		dataContainer = DataContainer.GetInstance();
		network = NetworkManager.GetInstance();
	}

}
