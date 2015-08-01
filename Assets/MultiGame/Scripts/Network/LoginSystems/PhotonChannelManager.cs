using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class PhotonChannelManager : Photon.MonoBehaviour {

	#region MemberVariables
	public string gameVersion = "0.0";
	public string characterName = "New Bee";
	public int maxRetries = 3;
	private int currentRetries;
	public int windowID = 934758;
	[System.NonSerialized]
	public int channel = 0;

	public int maxPerChannel = 32;
	public int idealPerChannel = 28;

	public enum InterfaceModes {Invisible, Collapsed, Login, Channel};
	[HideInInspector]
	public InterfaceModes interfaceMode = InterfaceModes.Login;
	[System.NonSerialized]
	public string targetScene;
	public bool useLegacyGUI = true;
	public Rect guiArea = new Rect(.01f,.01f,.2f,.98f);

	[System.NonSerialized]
	public string currentStatus = "";

	[System.NonSerialized]
	public List<RoomInfo> rooms = new List<RoomInfo>();
	private Vector2 scrollArea;


	public bool debug = false;

	#endregion

	#region MeatAndPotatoes

	void Awake () {
		targetScene = Application.loadedLevelName;
		currentRetries = maxRetries;
		if (PlayerPrefs.HasKey("characterName"))
			characterName = PlayerPrefs.GetString("characterName");
	}

	void OnGUI () {
		if (!useLegacyGUI || interfaceMode == InterfaceModes.Invisible)
			return;

		switch (interfaceMode) {
		case InterfaceModes.Login:
			GUILayout.Window(windowID, new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), LoginWindow, "Login");
			break;
		case InterfaceModes.Channel:
			GUILayout.Window(windowID, new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), ChannelWindow, "Channels");
			break;
		}
	}

	public void LoginWindow (int _id) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Character name: ");
		characterName = GUILayout.TextField(characterName);
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Join Game"))
			StartCoroutine(AttemptLogin());
	}

	public void ChannelWindow (int _id) {
		GUILayout.Label("Select a channel to join:");
		scrollArea = GUILayout.BeginScrollView(scrollArea, "box", GUILayout.ExpandHeight(true));

		if (rooms.Count > 0) {
			foreach (RoomInfo _rinfo in rooms) {
				GUILayout.BeginHorizontal();
				if (_rinfo.name.Contains(Application.loadedLevelName)) {
					if(!_rinfo.open || _rinfo.playerCount >= _rinfo.maxPlayers)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					if(GUILayout.Button(_rinfo.name,GUILayout.ExpandWidth(true)))
						StartCoroutine(SwapScene(_rinfo.name));
					GUILayout.Label("P " + _rinfo.playerCount + "/" + _rinfo.maxPlayers);
					GUILayout.EndHorizontal();
				}
			}
		}

		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	public void ConnectWithName (string _name) {
		characterName = _name;
		PlayerPrefs.SetString("characterName", characterName);
		PlayerPrefs.Save();
		StartCoroutine(AttemptLogin());
	}

	IEnumerator AttemptLogin () {
		PhotonNetwork.playerName = characterName;
		if (debug)
			Debug.Log("Attempting to connect to Photon...");
		yield return new WaitForEndOfFrame();
		PhotonNetwork.ConnectUsingSettings(gameVersion);
	}

	public IEnumerator SwapScene (string _targetScene) {
		targetScene = _targetScene;
		Debug.Log("Swap Scene initiated in room " + PhotonNetwork.room);
		if(PhotonNetwork.room != null)
			PhotonNetwork.LeaveRoom();
		
		PhotonNetwork.isMessageQueueRunning = false;
		yield return null;
		Debug.Log("Continued " + PhotonNetwork.room);

		Debug.Log("Loading... " + PhotonNetwork.room);
		Application.LoadLevel(_targetScene);
		yield return Application.isLoadingLevel;
		Debug.Log("Loaded scene. " + PhotonNetwork.room);
		
		if (PhotonNetwork.connectionState == ConnectionState.Connected) {
			if (PhotonNetwork.room == null) {
				RoomOptions options = new RoomOptions();
				options.isOpen = true;
				options.isVisible = true;//TODO: implement private channels
				PhotonNetwork.JoinOrCreateRoom(_targetScene + channel, options, TypedLobby.Default);
			}
		}
		else {
			Debug.LogWarning("But we were not connected!");
		}
	}

	#endregion

	#region PhotonCallbacks
	void OnFailedToConnectToPhoton () {
		if (currentRetries <= 0) {
			currentRetries = maxRetries;
			Debug.Log("Attempting reconnection");
		}
		else {
			currentRetries--;
			StartCoroutine(AttemptLogin());
			Debug.Log("Reconnect failed.");
		}
	}

	void OnConnectedToPhoton() {
		if (debug)
			Debug.Log("Connected to Photon.");
	}

	void OnJoinedLobby() {
		interfaceMode = InterfaceModes.Channel;
		rooms.Clear();
		rooms.AddRange( PhotonNetwork.GetRoomList());
	}

	#endregion
}
