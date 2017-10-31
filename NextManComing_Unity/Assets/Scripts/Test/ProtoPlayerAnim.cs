using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProtoPlayerAnim : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	public void Idle()
	{
		animator.SetBool("walk", false);
		animator.SetBool("run", false);
		animator.SetBool("back", false);
	}

	public void Walk()
	{
		animator.SetBool("walk", true);
		animator.SetBool("run", false);
	}

	public void Run()
	{
		animator.SetBool("walk", true);
		animator.SetBool("run", true);
	}

	public void Back()
	{
		animator.SetBool("back", true);
	}

	public void Attack()
	{
		animator.SetTrigger("attack");
	}
}
