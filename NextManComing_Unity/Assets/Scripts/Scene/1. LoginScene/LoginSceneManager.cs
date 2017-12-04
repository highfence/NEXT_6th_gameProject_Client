using HttpPacket;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
	private DataStorage    dataStorage;
	private NetworkManager network;
	private UISystem       uiSystem;

	[SerializeField]
	private string idInput;
	[SerializeField]
	private string pwInput;
	[SerializeField]
	private MessageBox msgBox;

	private void Awake()
	{
		dataStorage   = DataStorage.GetInstance();
		network       = NetworkManager.GetInstance();
		uiSystem      = FindObjectOfType<UISystem>();
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
		Assert.IsNotNull(idInputField);

		if (idInputField != null)
		{
			idInputField.name = "Id Input Field";
			idInputField.onValueChanged.AddListener(delegate { OnIdValueChanged(idInputField.text); });

			idInputField.text = "Type Your ID...";

			var fieldPosition = new Vector3()
			{
				x = Screen.width * 0.5f,
				y = Screen.height * 0.39f,
				z = 0
			};

			idInputField.transform.position = fieldPosition;

			uiSystem.AttachUI(idInputField.gameObject);
		}

		var pwInputField = Instantiate(Resources.Load("Prefabs/InputField") as GameObject).GetComponent<InputField>();
		Assert.IsNotNull(pwInputField);

		if (pwInputField != null)
		{
			pwInputField.name = "Pw Input Field";
			pwInputField.onValueChanged.AddListener(delegate { OnPwValueChanged(pwInputField.text); });

			pwInputField.text = "Type Your Password...";

			var fieldPosition = new Vector3()
			{
				x = Screen.width * 0.5f,
				y = Screen.height * 0.29f,
				z = 0
			};

			pwInputField.transform.position = fieldPosition;

			uiSystem.AttachUI(pwInputField.gameObject);
		}

		#endregion

		#region BUTTON FIELD INITIALIZE

		var loginButton = Instantiate(Resources.Load("Prefabs/Button") as GameObject).GetComponent<Button>();
		Assert.IsNotNull(loginButton);

		if (loginButton != null)
		{
			loginButton.name = "Login Button";
			loginButton.onClick.AddListener(OnLoginButtonClicked);

			var buttonText = loginButton.gameObject.GetComponentInChildren<Text>();
			Assert.IsNotNull(buttonText);
			buttonText.text = "LOGIN";

			var buttonPosition = new Vector3()
			{
				x = Screen.width * 0.5f,
				y = Screen.height * 0.15f,
				z = 0
			};

			loginButton.transform.position = buttonPosition;

			uiSystem.AttachUI(loginButton.gameObject);
		}

		#endregion

		#region MSG BOX INITIALIZE

		Assert.IsNotNull(msgBox);

		msgBox.Hide();

		#endregion
	}



	#region CALLBACK METHODS

	private void OnIdValueChanged(string changedValue)
	{
		idInput = changedValue;
	}

	private void OnPwValueChanged(string changedValue)
	{
		pwInput = changedValue;
	}

	private void OnLoginButtonClicked()
	{
		// 패스워드와 비밀번호는 null이거나 비어있을 수 없다.
		if (string.IsNullOrEmpty(idInput) || string.IsNullOrEmpty(pwInput))
		{
			Debug.LogAssertion("Login Failed. ID / PW is Null or Empty");

			msgBox.Show("Login failed. \n Please checkout ID & PW written properly");

			return;
		}

		Debug.Log($"Login Button Clicked. Id({idInput}), Pw({pwInput})");

		dataStorage.LoginInfoStore(idInput, pwInput);

		var loginReq = new LoginReq()
		{
			UserId = idInput,
			UserPw = pwInput
		};

		var reqUrl = dataStorage.Config.GetUri() + "Login/Login";

		Debug.Log($"Login req url : {reqUrl}");

		network.HttpPost<LoginReq, LoginRes>(reqUrl, loginReq, OnLoginResultArrived);
	}

	// 로그인 요청에 대한 답변이 왔을 경우 호출되는 콜백 메서드.
	private bool OnLoginResultArrived(LoginRes response)
	{
		if (response.Result == 0)
		{
			dataStorage.LoginResultStore(response);

			Debug.Log($"Login Result Arrived. Token({response.Token})");

			network.TcpConnect(response.ManageServerAddr, response.ManageServerPort);

			SceneManager.LoadScene("2. Server Scene");

			return true;
		}
		else
		{
			msgBox.Show("Login failed. \n Please checkout ID & Pw written properly");
			return false;
		}
	}

	#endregion
}
