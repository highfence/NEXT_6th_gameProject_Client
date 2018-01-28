using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Invoke events for several inputs which ingame objects are interested.
public class GameInputLayer : MonoBehaviour
{
	#region INPUT EVENTS

	public Action<bool> OnEnterKeyPressed = delegate { };

	#endregion

	// Save enter key status for player controllable or UI visibility.
	[SerializeField]
	public bool IsEnterPressed = false;

	// Get Key Inputs.
	private void Update()
	{
		WatchEnterKeyStatus();	
	}

	// Watching enter key status and invoke event.
	private void WatchEnterKeyStatus()
	{
		// Return immediately when enter key status is not changed.
		if (Input.GetKeyDown(KeyCode.Return) == false)
			return;

		// Change enter key status.
		if (IsEnterPressed)
		{
			IsEnterPressed = false;
		}
		else
		{
			IsEnterPressed = true;
		}

		// Invoke event with changed enter key status.
		OnEnterKeyPressed.Invoke(IsEnterPressed);
	}
}
