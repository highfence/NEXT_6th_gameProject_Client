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
	public bool IsUIOpened = false;

	public void Start()
	{
		transform.name = "Player";
		originalRotation = transform.localRotation;
	}


	public void Update()
	{
		if (isPlayerControllable())
		{
			RotatePlayer();

			MovePlayer();

			AttackPlayer();
		}
	}

	private bool isPlayerControllable()
	{
		if (IsUIOpened)
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
	public float rotAverageX = 0f;
	public float rotationX = 0f;
	private List<float> rotArrayX = new List<float>();
	private float frameCounter = 20;
	private float sensitivityX = 15f;
	private float minimumX = -360f;
	private float maximumX = 360f;
	private Quaternion originalRotation;

	private void RotatePlayer()
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
		rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);
		Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
		transform.localRotation = originalRotation * xQuaternion;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F))
		{
			if (angle < -360F)
			{
				angle += 360F;
			}
			if (angle > 360F)
			{
				angle -= 360F;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	private float AngleBetweenPoints(Vector2 a, Vector2 b)
	{
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
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
