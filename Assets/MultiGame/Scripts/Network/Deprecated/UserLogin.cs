using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserLogin : Photon.MonoBehaviour {

	public string gameVersion = "0.0";
	public Rect guiArea;
	public int windowID = 0928374;
	private bool showLogin = true;
	public bool showChannelWindow = false;
//	public bool autoJoin = true;
	[System.NonSerialized]
	public bool indieFriendlySceneLoad = false;

	[HideInInspector]
	public string alias = "New Bee";

	public int idealPlayerCount = 28;
	public int maxPlayerCount = 32;

	public bool debug = false;

	[HideInInspector]
	public bool loggedIn = false;
	private bool sentMessage = false;

	private bool signupError = false;
	private bool loginError = false;

	private string username = "";
	private string password = "";
	private bool canLogin = false;

	private string channelName = "";
	private bool jumpToTarget = false;
	private string targetScene = "";

	private bool photonInitialized = false;
	private int failCount  = 0;

	private bool loadingAsync = false;
	private List<RoomInfo> roomList = new List<RoomInfo>();
	private Vector2 scrollArea;

	[HideInInspector]
	public float creationTime;

	public bool dontDestOnLoad = true;

	void Awake () {
		creationTime = Time.time;
		if (dontDestOnLoad)
			DontDestroyOnLoad(gameObject);
		sentMessage = false;
		loggedIn = false;

		if (PlayerPrefs.HasKey("username"))
			username = PlayerPrefs.GetString("username");
		if (PlayerPrefs.HasKey("alias"))
			alias = PlayerPrefs.GetString("alias");

		if (PlayerPrefs.HasKey("channelName"))
			channelName = PlayerPrefs.GetString("channelName");
		else
			channelName = "0";

	}

	void Start () {
		UserLogin[] userLoginManagers = FindObjectsOfType<UserLogin>();
		foreach(UserLogin man in userLoginManagers) {
			if (man.creationTime < creationTime) {
				Destroy(gameObject);
				enabled = false;
				return;
			}
		}
	}

	void OnGUI () {
		if (loggedIn && !sentMessage) {
			gameObject.BroadcastMessage("LoggedIn", SendMessageOptions.DontRequireReceiver);
			sentMessage = true;
		}
		if (!loggedIn | showLogin)
			GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), LoginWindow, "Login:");
		else if (loggedIn && showChannelWindow)
			GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), ChannelWindow, "Channel:");
		else {
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), "", "");
			if(GUILayout.Button("Channels"))
				showChannelWindow = !showChannelWindow;
			GUILayout.EndArea();
		}



//		if (!PhotonNetwork.connected) {
//			GUILayout.Label("Connecting...",GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//			if (!photonInitialized) {
//				photonInitialized = true;
//				PhotonLogin();	
//			}
//			return;
//		}
	}

	//lets the user select which group of players to play the game with
	//If the first character in server name is a # the server is public.
	void ChannelWindow (int id) {
		if(GUILayout.Button("Channels"))
			showChannelWindow = !showChannelWindow;
		GUILayout.Label("" + PhotonNetwork.connectionState);
		if (PhotonNetwork.room != null)
			GUILayout.Label(PhotonNetwork.room.name);
		if (PhotonNetwork.insideLobby && !jumpToTarget) {
			scrollArea = GUILayout.BeginScrollView(scrollArea,"box");
			if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true))) {
				roomList.Clear();
				roomList.AddRange( PhotonNetwork.GetRoomList());
			}
			
			if (roomList.Count > 0) {
				foreach (RoomInfo rinfo in roomList) {
					GUILayout.BeginHorizontal();
					if (rinfo.name.Contains(Application.loadedLevelName)) {
						if(!rinfo.open || rinfo.playerCount >= rinfo.maxPlayers)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						if(GUILayout.Button(rinfo.name,GUILayout.ExpandWidth(true)))
							StartCoroutine(SwapScene(rinfo.name));
						GUILayout.Label("P " + rinfo.playerCount + "/" + rinfo.maxPlayers);
						GUILayout.EndHorizontal();
					}
				}
			}

			GUI.color = Color.white;
			GUILayout.EndScrollView();
		}
		
//		GUILayout.BeginHorizontal();
//		GUILayout.Label("Join Automatically?",GUILayout.ExpandWidth(true));
//		GUILayout.FlexibleSpace();
//		//autoJoin = GUILayout.Toggle(autoJoin, "",GUILayout.ExpandWidth(true));
//		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Channel : ");
		//GUILayout.FlexibleSpace();
		channelName = GUILayout.TextField(channelName, 8, GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal();

		//GUILayout.FlexibleSpace();

		if ( PhotonNetwork.room != null && (GUILayout.Button("Leave Channel",GUILayout.ExpandWidth(true))))
			PhotonNetwork.LeaveRoom();
		if (PhotonNetwork.insideLobby) {
			if (GUILayout.Button("Go to channel " + channelName,GUILayout.ExpandWidth(true))) {

				StartCoroutine(SwapScene(Application.loadedLevelName));
			}
		}
		GUILayout.Label("For channel, enter a number (0-100) for public channels, or the name if you know of a private channel here. To play with friends, meet at the same place in the same channel.");

	}

	void LoginWindow (int id) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Character Name: ");
		alias = GUILayout.TextField(alias, 24);
		GUILayout.EndHorizontal();

//		GUILayout.BeginHorizontal();
//		GUILayout.Label("Email: ");
//		username = GUILayout.TextField(username, GUILayout.ExpandWidth(true));
//		if (username == "")
//			canLogin = false;
//		if (username != "" && !username.Contains("@"))
//			canLogin = false;
//		else {
//			if (password != "")
//				canLogin = true;
//		}
//		GUILayout.EndHorizontal();

		//TODO: fix temporary hack for anonymous logins
		if (!string.IsNullOrEmpty(alias))
			canLogin = true;
		else
			canLogin = false;

//		GUILayout.BeginHorizontal();
//		GUILayout.Label("Password: ");
//		password = GUILayout.PasswordField(password, '*', GUILayout.ExpandWidth(true));
//		GUILayout.EndHorizontal();
//
//		if (signupError)
//			GUILayout.Label("Username is already in use.", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//		if (loginError)
//			GUILayout.Label("Login failed. Check username and password and try again.");

		GUILayout.BeginHorizontal();
		if (canLogin) {
//			if (GUILayout.Button("Sign Up"))
//				AttemptSignup();
			if (GUILayout.Button("Log In") || Input.GetKeyUp(KeyCode.Return)) {
				if (debug)
					Debug.Log("User clicked login button");
				AttemptLogin();
			}
		}

		GUILayout.EndHorizontal();

	}

	void AttemptLogin () {
		PlayerPrefs.SetString("username", username);
		PlayerPrefs.SetString("alias", alias);
		PlayerPrefs.Save();

		//TODO: Securely log-in the user

		PhotonLogin();

	}

	void PhotonLogin () {
		if (debug) Debug.Log("Login initiated at " + Time.time);
		photonInitialized = true;
		PhotonNetwork.ConnectUsingSettings(gameVersion);
	}

	void OnConnectedToPhoton () {
		if (debug) Debug.Log("Connected at " + Time.time);
		PhotonNetwork.playerName = alias;
		loggedIn = true;
		//StartCoroutine(SwapScene(Application.loadedLevelName));
		HideLogin();
		ShowChannels();
	}

	void OnDisconnectedFromPhoton () {
		if (debug) Debug.Log("Disconnected at " + Time.time);
		photonInitialized = false;
	}

	IEnumerator OnJoinedLobby () {
		if (jumpToTarget) {
			Debug.Log("Jumping to a target scene and channel...");
			RoomOptions options = new RoomOptions();
			options.isOpen = true;
			options.isVisible = StartsWithNumber(channelName);
			yield return PhotonNetwork.JoinOrCreateRoom(Application.loadedLevelName + channelName, options, TypedLobby.Default);
		}
		else
			showChannelWindow = true;

		if (debug) Debug.Log("Joined lobby at " + Time.time);
//		if (autoJoin)
//			JoinIdealRoom ();
//		autoJoin = false;
	}

	void JoinIdealRoom () {
		if (!PhotonNetwork.insideLobby) {
			PhotonNetwork.LeaveRoom();
			return;
		}
		if (PhotonNetwork.countOfRooms == 0) {
			CreateNewChannel(Application.loadedLevelName + "0");
			return;
		}
		if (PhotonNetwork.countOfRooms == 1) {
			PhotonNetwork.JoinRandomRoom();
			return;
		}
		if (PhotonNetwork.countOfRooms > 1) {
			RoomInfo[] _roomInfo = PhotonNetwork.GetRoomList();
			foreach (RoomInfo rinfo in _roomInfo) {
				if (rinfo.playerCount < idealPlayerCount && rinfo.name.Contains(Application.loadedLevelName)) {
					PhotonNetwork.JoinRoom(rinfo.name);
					return;
				}
			}

		}
	}

	public IEnumerator SwapScene (string _targetScene) {
		targetScene = _targetScene;
		Debug.Log("Swap Scene initiated in room " + PhotonNetwork.room);
//		autoJoin = false;
		jumpToTarget = true;
		if(PhotonNetwork.room != null)
			PhotonNetwork.LeaveRoom();

		PhotonNetwork.isMessageQueueRunning = false;
		yield return null;
		Debug.Log("Continued " + PhotonNetwork.room);

		if( !indieFriendlySceneLoad) {
			AsyncOperation asyncLoad = Application.LoadLevelAsync(_targetScene);
			loadingAsync = true;
			Debug.Log("Loading... " + PhotonNetwork.room);
			yield return asyncLoad;
			PhotonNetwork.isMessageQueueRunning = true;
			
			Debug.Log("Loaded scene. " + PhotonNetwork.room);
			loadingAsync = false;
		}
		else {
			Debug.Log("Loading... " + PhotonNetwork.room);
			loadingAsync = true;
			Application.LoadLevel(_targetScene);
			yield return Application.isLoadingLevel;
			loadingAsync = false;
			Debug.Log("Loaded scene. " + PhotonNetwork.room);
		}

		if (PhotonNetwork.connectionState == ConnectionState.Connected) {
			if (PhotonNetwork.room == null) {
				RoomOptions options = new RoomOptions();
				options.isOpen = true;
				options.isVisible = StartsWithNumber(channelName);
				PhotonNetwork.JoinOrCreateRoom(_targetScene + channelName, options, TypedLobby.Default);
			}
		}
		else {
			Debug.LogWarning("But we were not connected!");
			loggedIn = false;//uh-oh. Let the Game UI handle the solution.
		}
	}

//	public IEnumerator SwapChannel (string targetChannel) {
//
//		yield return StartCoroutine(SwapScene(Application.loadedLevelName));
////		if (PhotonNetwork.room == null)
////			PhotonNetwork.JoinRoom(targetChannel);
//
//	}

	void OnPhotonJoinRoomFailed () {
		Debug.Log("Failed to join room...");
		failCount++;
		if (failCount < 3) {
			//IncrementChannel ();
			StartCoroutine(SwapScene(Application.loadedLevelName));
		}
		else {
			IncrementChannel ();
			StartCoroutine(SwapScene(Application.loadedLevelName));
			if (failCount > 10) {
				CreateNewChannel(Application.loadedLevelName + channelName);
				failCount = 0;
				return;
			}
		}
	}

	void OnJoinedRoom () {
		showChannelWindow = false;
		if (jumpToTarget) {
			jumpToTarget = false;
		}

		if ((Application.loadedLevelName + channelName) != targetScene + channelName) {
			Debug.Log("Joined room, changing scene.");
			StartCoroutine(SwapScene(targetScene));
		}

		if (debug) Debug.Log("Joined " + PhotonNetwork.room.name + " at " + Time.time);
		failCount = 0;
	}

	void OnPhotonCreateRoomFailed () {
		Debug.Log("Failed to create room...");
		failCount++;
		if (failCount > 3)
			CreateNewChannel(Application.loadedLevel + channelName);
	}

	void IncrementChannel () {
		string _newName = "";
		int _newNumber;
		if (StartsWithNumber(channelName)) {
			for (int i = 0; i < channelName.Length; i++) {
				if (IsNumber(channelName[i]))
					_newName = _newName + channelName[i];
				else {
					channelName = channelName.Remove(0, _newName.Length);
					_newNumber = System.Convert.ToInt32(_newName) + 1;
					channelName = channelName + _newNumber.ToString();
				}
			}

		}
	}

	//TODO: Implement connection failure and retries (offline mode?)
//	void OnFailedToConnectToPhoton () {
//
//	}

	void CreateNewChannel (string targetChannel) {
		if (StartsWithNumber(channelName))
			PhotonNetwork.CreateRoom(targetChannel, true, true, maxPlayerCount);
		else
			PhotonNetwork.CreateRoom(targetChannel, false, true, maxPlayerCount);
	}

	void AttemptSignup () {
		//TODO: Securely sign-up a new user
	}

	private bool StartsWithNumber (string _channelName) {
		return IsNumber(_channelName[0]);
	}

	private bool IsNumber (char ch) {
		bool ret = false;
		switch (ch){
		case '0':
			ret = true;
			break;
		case '1':
			ret = true;
			break;
		case '2':
			ret = true;
			break;
		case '3':
			ret = true;
			break;
		case '4':
			ret = true;
			break;
		case '5':
			ret = true;
			break;
		case '6':
			ret = true;
			break;
		case '7':
			ret = true;
			break;
		case '8':
			ret = true;
			break;
		case '9':
			ret = true;
			break;
		}
		return ret;
	}

	public void Logout () {
		//TODO: Allow the user to log-out of the current session
	}

	public void ShowLogin () {
		showLogin = true;
	}

	public void HideLogin () {
		showLogin = false;
	}

	public void ShowChannels () {
		showChannelWindow = true;
	}

	public void HideChannels () {
		showChannelWindow = false;
	}

}
//Copyright 2014 William Hendrickson all rights reserved.
