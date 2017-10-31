using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class ProtoPlayerMove : MonoBehaviour
{
	[SerializeField]
	private CharacterController characterController;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AnimatorStateInfo currentState;

	[SerializeField]
	private AnimatorStateInfo prevState;

	[SerializeField]
	private Transform cameraTransform;

	[SerializeField]
	private float moveSpeed = 10.0f;

	public void Start()
	{
		animator = GetComponent<Animator>();
		currentState = animator.GetCurrentAnimatorStateInfo(0);
		prevState = currentState;
	}

	public void Update()
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Vector3 moveDirection = new Vector3(x, 0, z);
		moveDirection = cameraTransform.TransformDirection(moveDirection);
		moveDirection *= moveSpeed;

		characterController.Move(moveDirection * Time.deltaTime);
	}
}
