using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수학적인 함수들을 모아놓는 유틸 클래스.
public class MathUtil 
{
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

	public static float AngleBetweenPoints(Vector2 a, Vector2 b)
	{
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}
}
