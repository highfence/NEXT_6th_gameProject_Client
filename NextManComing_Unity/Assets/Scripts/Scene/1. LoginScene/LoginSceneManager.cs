using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
	private DataContainer dataContainer;
	private NetworkManager network;
	private UISystem uiSystem;

	[SerializeField]
	private string idInput;
	[SerializeField]
	private string pwInput;

	private void Awake()
	{
		dataContainer = DataContainer.GetInstance();
		network = NetworkManager.GetInstance();
		uiSystem = FindObjectOfType<UISystem>();
	}

	private void Start()
	{
		UIInitialize();
	}

	// UI 오브젝트들을 초기화시켜주는 메서드.
	private void UIInitialize()
	{
		Assert.IsNotNull(uiSystem);

		#region INPUT FIELD INITIALIZE

		var idInputField = Instantiate(Resources.Load("Prefabs/InputField") as GameObject).GetComponent<InputField>();

		if (idInputField != null)
		{
			idInputField.name = "Id Input Field";
			idInputField.onValueChanged.AddListener(delegate { IdValueChangeCheck(idInputField.text); });

			idInputField.text = "Type Your ID...";

			var fieldPosition = new Vector3()
			{
				x = Screen.width * 0.5f,
				y = Screen.height * 0.35f,
				z = 0
			};

			idInputField.transform.position = fieldPosition;

			uiSystem.AttachUI(idInputField.gameObject);
		}

		var pwInputField = Instantiate(Resources.Load("Prefabs/InputField") as GameObject).GetComponent<InputField>();

		if (pwInputField != null)
		{
			pwInputField.name = "Pw Input Field";
			pwInputField.onValueChanged.AddListener(delegate { PwValueChangeCheck(pwInputField.text); });

			pwInputField.text = "Type Your Password...";

			var fieldPosition = new Vector3()
			{
				x = Screen.width * 0.5f,
				y = Screen.height * 0.25f,
				z = 0
			};

			pwInputField.transform.position = fieldPosition;

			uiSystem.AttachUI(pwInputField.gameObject);
		}

		#endregion


	}

	private void IdValueChangeCheck(string changedValue)
	{
		idInput = changedValue;
	}

	private void PwValueChangeCheck(string changedValue)
	{
		pwInput = changedValue;
	}

}
