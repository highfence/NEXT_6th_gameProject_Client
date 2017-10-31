using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoNpc : MonoBehaviour
{
	[SerializeField]
	private int hp = 5;

	[SerializeField]
	private Animator npcAnim;

	void Start ()
	{
		gameObject.name = "npc";
	}
	
	void Update ()
	{
		
	}

	private void Damage(Vector3 playerPosition)
	{
		hp -= 1;

		transform.LookAt(playerPosition);

		if (hp <= 0)
		{
			npcAnim.SetTrigger("death");
		}
		else
		{
			npcAnim.SetTrigger("damage");
		}
	}
}
