using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProtoCharacterAnim : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AnimatorStateInfo currentState;

	[SerializeField]
	private AnimatorStateInfo prevState;

	public void Start()
	{
		animator = GetComponent<Animator>();
		currentState = animator.GetCurrentAnimatorStateInfo(0);
		prevState = currentState;
	}

	public void Update()
	{
		
	}
}
