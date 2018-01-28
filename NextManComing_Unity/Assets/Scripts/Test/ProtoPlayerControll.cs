using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ProtoPlayerControll : MonoBehaviour
{
	[SerializeField]
	private CharacterController characterController;

	[SerializeField]
	private ProtoPlayerAnim playerAnim;

	[SerializeField]
	private Transform cameraTransform;

	[SerializeField]
	private const float walkSpeed = 5.0f;

	[SerializeField]
	private const float runSpeed = 10.0f;

	[SerializeField]
	public bool IsAttacking = false;

	[SerializeField]
	public bool IsEnterPressed = false;

	public void Start()
	{
		transform.name = "Player";

		RegistInputEvents();

		originalRotation = transform.localRotation;
	}

	public void Update()
	{
		if (isPlayerControllable())
		{
			MovePlayer();

			AttackPlayer();
		}
	}

	// Regist events to game input layer.
	private void RegistInputEvents()
	{
		var gameInputLayer = FindObjectOfType<GameInputLayer>();

		if (gameInputLayer == null)
		{
			Debug.LogAssertion("There is no game input layer in this scene. (Msg from player controller initialize)");
			return;
		}

		// Sync enter key status with input layer.
		gameInputLayer.OnEnterKeyPressed += enterKeyStatus => { IsEnterPressed = enterKeyStatus; };
		gameInputLayer.OnMouseRotate += RotatePlayer;
	}

	private bool isPlayerControllable()
	{
		if (IsEnterPressed)
			return false;

		return true;
	}

	private void MovePlayer()
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		if (x != 0.0f || z != 0.0f)
		{
			float moveSpeed;

			if (Input.GetKey(KeyCode.S))
			{
				moveSpeed = walkSpeed;
				playerAnim.Walk();
			}
			else if (Input.GetKey(KeyCode.LeftShift))
			{
				moveSpeed = runSpeed;
				playerAnim.Run();
			}
			else
			{
				moveSpeed = walkSpeed;
				playerAnim.Walk();
			}

			Vector3 moveDirection = new Vector3(x, 0, z);
			moveDirection = cameraTransform.TransformDirection(moveDirection);
			moveDirection *= moveSpeed;

			characterController.Move(moveDirection * Time.deltaTime);
		}
		else
		{
			playerAnim.Idle();
		}
	}

	// For Rotate
	private Quaternion originalRotation;

	// Rotate player accord with input layer data.
	private void RotatePlayer(Quaternion xRotateQuaternion)
	{
		if (isPlayerControllable() == false) return;

		transform.localRotation = originalRotation * xRotateQuaternion;
	}

	private void AttackPlayer()
	{
		if (Input.GetMouseButtonDown(0))
		{
			IsAttacking = true;
			playerAnim.Attack();
		}
	}

	public void EndAttack()
	{
		IsAttacking = false;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (IsAttacking == true && hit.gameObject.name == "npc")
		{
			IsAttacking = false;
			hit.gameObject.SendMessage("Damage", transform.position);
			Debug.Log("Damage!");
		}
	}
}
