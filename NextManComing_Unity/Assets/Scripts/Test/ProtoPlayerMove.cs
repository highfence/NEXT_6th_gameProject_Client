using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ProtoPlayerMove : MonoBehaviour
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

	public void Start()
	{

	}

	public void Update()
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		if (x != 0.0f || z != 0.0f)
		{
			float moveSpeed;

			if (Input.GetKey(KeyCode.LeftShift))
			{
				moveSpeed = runSpeed;
			}
			else
			{
				moveSpeed = walkSpeed;
			}

			Vector3 moveDirection = new Vector3(x, 0, z);
			moveDirection = cameraTransform.TransformDirection(moveDirection);
			moveDirection *= moveSpeed;

			characterController.Move(moveDirection * Time.deltaTime);
		}
		else
		{

		}
	}
}
