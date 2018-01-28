using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Invoke events for several inputs which ingame objects are interested.
public class GameInputLayer : MonoBehaviour
{
	#region INPUT EVENTS

	public Action<bool>		  OnEnterKeyPressed = delegate { };
	public Action<Quaternion> OnMouseRotate		= delegate { };
	public Action			  OnAttackPressed   = delegate { };

	#endregion

	// Save enter key status for player controllable or UI visibility.
	[SerializeField]
	public bool IsEnterPressed = false;

	// Watch inputs and make events.
	private void Update()
	{
		WatchEnterKeyStatus();

		WatchMouseXRotate();

		WatchAttackKeyStatus();
	}

	private void WatchAttackKeyStatus()
	{
		if (Input.GetMouseButtonDown(0) == false) return;

		OnAttackPressed.Invoke();
	}

	// Watching enter key status and invoke event.
	private void WatchEnterKeyStatus()
	{
		// Return immediately when enter key status is not changed.
		if (Input.GetKeyDown(KeyCode.Return) == false) return;

		// Change enter key status.
		if (IsEnterPressed) IsEnterPressed = false;
		else				IsEnterPressed = true;

		// Invoke event with changed enter key status.
		OnEnterKeyPressed.Invoke(IsEnterPressed);
	}

	#region FOR MOUSE ROTATE

	public float rotAverageX = 0f;
	public float rotationX = 0f;
	private List<float> rotArrayX = new List<float>();
	private float frameCounter = 20;
	private float sensitivityX = 5f;
	private float minimumX = -360f;
	private float maximumX = 360f;

	#endregion

	private void WatchMouseXRotate()
	{
		rotAverageX = 0f;
		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		rotArrayX.Add(rotationX);

		if (rotArrayX.Count >= frameCounter)
		{
			rotArrayX.RemoveAt(0);
		}

		for (int i = 0; i < rotArrayX.Count; i++)
		{
			rotAverageX += rotArrayX[i];
		}

		rotAverageX /= rotArrayX.Count;
		rotAverageX = MathUtil.ClampAngle(rotAverageX, minimumX, maximumX);

		Quaternion xRotateQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

		OnMouseRotate.Invoke(xRotateQuaternion);
	}
}
