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
	private bool		   isConnectTrying = false;
	private InputField     idInputField;
	private InputField     pwInputField;

	[SerializeField]
	private string idInput;

	[SerializeField]
	private string pwInput;

	[SerializeField]
	private MessageBox msgBox;


	/// <summary>
	/// LoginScene에 들어오자마자 초기화를 진행해주는 메서드.
	/// 주로 그 전 씬에 남아있던 싱글톤 컨텍스트 객체들을 호출한다.
	/// </summary>
	private void Awake()
	{
		dataStorage   = DataStorage.GetInstance();
		network       = NetworkManager.GetInstance();
		uiSystem      = FindObjectOfType<UISystem>();
	}


	/// <summary>
	/// Awake에서 진행된 뒤에 초기화가 필요한 것은 이 곳에서 진행.
	/// </summary>
	private void Start()
	{
		UIInitialize();
	}


	/// <summary>
	/// UI 관련 오브젝트들의 초기화를 맡아주는 메서드,
	/// TODO :: 현재 메시지 박스가 IdInputField, PwInputField 보다 후 순위라서 이를 고쳐주어야 함.
	/// </summary>
	private void UIInitialize()
	{
		Assert.IsNotNull(uiSystem);

		#region INPUT FIELD INITIALIZE

		idInputField = Instantiate(Resources.Load("Prefabs/InputField") as GameObject).GetComponent<InputField>();
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

		pwInputField = Instantiate(Resources.Load("Prefabs/InputField") as GameObject).GetComponent<InputField>();
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


	/// <summary>
	/// Id 값이 바뀔 경우 그 값을 저장하는 메서드.
	/// TODO :: 한 번 커넥트를 시도하여 네트워크 IO가 작업중인 경우에는 그 값을 바꾸지 않도록 조정해야 함.
	/// </summary>
	/// <param name="changedValue"></param>
	private void OnIdValueChanged(string changedValue)
	{
		if (isConnectTrying == false)
		{
			idInput = changedValue;
		}
	}


	/// <summary>
	/// 패스워드 값이 바뀔 경우 그 값을 저장하는 메서드.
	/// </summary>
	/// <param name="changedValue"></param>
	private void OnPwValueChanged(string changedValue)
	{
		if (isConnectTrying == false)
		{
			pwInput = changedValue;
		}
	}


	/// <summary>
	/// 로그인 버튼을 클릭하였을 경우 이를 처리해주는 메서드.
	/// </summary>
	private void OnLoginButtonClicked()
	{

#if DEBUG
		if (string.IsNullOrEmpty(idInput) || string.IsNullOrEmpty(pwInput))
		{
			SceneManager.LoadScene("3. Lobby");
			return;
		}
#endif

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


	/// <summary>
	/// 로그인 요청에 대한 답변이 도착하였을 경우 호출되는 콜백 메서드.
	/// </summary>
	/// <param name="response"></param>
	/// <returns></returns>
	private bool OnLoginResultArrived(LoginRes response)
	{
		if (response.Result == 0)
		{
			dataStorage.LoginResultStore(response);

			Debug.Log($"Login Result Arrived. Token({response.Token})");

			// 받은 결과를 토대로 접속을 미리 시도해 놓는다.
			// Server Scene에서 로딩을 최대한 줄이기 위하여.
			network.TcpConnect(response.ManageServerAddr, response.ManageServerPort);

			SceneManager.LoadScene("3. Lobby");

			return true;
		}
		else
		{
			// 결과가 올바르지 않으면 메시지 박스를 띄워준다.
			// TODO :: Result 분석하여 때에 따른 메시지 박스를 호출할 수 있도록 해주어야 함.
			msgBox.Show("Login failed. \n Please checkout ID & Pw written properly");
			isConnectTrying = false;

			OnIdValueChanged(idInputField.text);
			OnPwValueChanged(pwInputField.text);

			return false;
		}
	}

	#endregion
}
