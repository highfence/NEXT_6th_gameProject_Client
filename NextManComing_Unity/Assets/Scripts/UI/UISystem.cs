using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// 각 씬의 UI 오브젝트를 래핑하는 클래스.

public class UISystem : MonoBehaviour
{
	[SerializeField]
	public Camera UICam;

	[SerializeField]
	public Canvas UICanvas;

	private void Start()
	{
		// 필요한 컴포넌트 들이 잘 매핑 되어있는지 검사.
		Assert.IsNotNull(UICam);
		Assert.IsNotNull(UICanvas);
	}

	// UI 시스템에 만들어진 UI 오브젝트를 등록하는 메서드.
	public void AttachUI(GameObject uiObject)
	{
		Assert.IsNotNull(uiObject);

		uiObject.transform.SetParent(UICanvas.transform);
	}
}
