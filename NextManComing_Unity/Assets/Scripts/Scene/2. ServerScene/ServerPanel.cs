using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerPanel : MonoBehaviour
{
	[SerializeField]
	Text nameText;

	[SerializeField]
	Text countText;

	private string Address;

	public void SetInfo(string name, int count, string address)
	{
		nameText.text = name;
		countText.text = count.ToString() + " / 8000";
		Address = address;
	}
}
