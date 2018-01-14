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

	private void RotatePlayer()
	{
		Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

		//Get the Screen position of the mouse
		Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

		//Get the angle between the points
		float angle = AngleBetweenPoints(positionOnScreen, mouseOnScreen);

				
		transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
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
