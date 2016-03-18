using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using MultiGame;

namespace MultiGame {
	[RequireComponent(typeof(Persistent))]
	public class PhotonChannelManager : Photon.MonoBehaviour {

		#region MemberVariables
		public string gameVersion = "0.0";
		public string characterName = "New Bee";
		public int maxRetries = 3;
		public MessageManager.ManagedMessage connectFailedMessage;
		public MessageManager.ManagedMessage disconnectedMessage;
		private int currentRetries;
		public int windowID = 934758;
		[System.NonSerialized]
		public int channel = 0;

		public int maxPerChannel = 32;
		public int idealPerChannel = 28;

		public enum InterfaceModes {Collapsed, Login, Channel};
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

			PhotonChannelManager[] _managers = FindObjectsOfType<PhotonChannelManager>();
			if (_managers.Length > 1) {
				Debug.LogError("Photon Channel Manager " + gameObject.name + " has detected that there is more than one in the scene. Please make sure there is always exactly one Photon Channel Manager, " +
					"added in one of the first scenes in your game.");
			}
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref connectFailedMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref disconnectedMessage, gameObject);
		}

		void OnGUI () {
			if (!useLegacyGUI)
				return;
			
			switch (interfaceMode) {
			default:
				return;
			case InterfaceModes.Login:
				GUILayout.Window(windowID, new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), LoginWindow, "Login");
				break;
			case InterfaceModes.Channel:
				GUILayout.Window(windowID, new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), ChannelWindow, "Channels");
				break;
			case InterfaceModes.Collapsed:
				GUILayout.Window(windowID, new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), CollapsedWindow, "Channels");
				break;
			}
		}

		private void CollapsedWindow (int _id) {
			GUILayout.BeginVertical("box");
			if (GUILayout.Button("Drop to Lobby"))
				PhotonNetwork.LeaveRoom();
			GUILayout.EndVertical();

		}

		private void LoginWindow (int _id) {
			GUILayout.BeginVertical("box");
			GUILayout.Label("Character name: ");
			characterName = GUILayout.TextField(characterName);
			if (GUILayout.Button("Join Game"))
				ConnectWithName(characterName);
			GUILayout.EndVertical();
		}

		private void ChannelWindow (int _id) {
			scrollArea = GUILayout.BeginScrollView(scrollArea, "box", GUILayout.ExpandHeight(true));

			if (PhotonNetwork.insideLobby && rooms.Count > 0) {
				GUILayout.Label("Select a channel to join:");
				foreach (RoomInfo _rinfo in rooms) {
					GUILayout.BeginHorizontal("box", GUILayout.Height(64f));
					if (_rinfo.name.Contains(Application.loadedLevelName)) {
						if(!_rinfo.open || _rinfo.playerCount >= _rinfo.maxPlayers)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						if(GUILayout.Button(_rinfo.name,GUILayout.ExpandWidth(true)))
							StartCoroutine(SwapScene(_rinfo.name));
						GUILayout.Label("P " + _rinfo.playerCount + "/" + _rinfo.maxPlayers);
					}
					GUILayout.EndHorizontal();
				}
			} else {
				if (GUILayout.Button("Join", GUILayout.Height(64f))) {
					RoomOptions options = new RoomOptions();
					options.isOpen = true;
					options.isVisible = true;//TODO: implement private channels
					PhotonNetwork.JoinOrCreateRoom(targetScene + channel, options, TypedLobby.Default);
				}

			}
				
			GUILayout.EndScrollView();
		}

		public void Connect() {
			ConnectWithName(characterName);
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

		private IEnumerator SwapScene (string _targetScene) {
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
			targetScene = Application.loadedLevelName;
			
			if (PhotonNetwork.connectionState == ConnectionState.Connected) {
				if (PhotonNetwork.room == null) {
					RoomOptions options = new RoomOptions();
					options.isOpen = true;
					options.isVisible = true;//TODO: implement private channels
					PhotonNetwork.JoinOrCreateRoom(_targetScene + channel, options, TypedLobby.Default);
				}
			}
			else {
				Debug.Log("But we were not connected!");
			}
		}

		#endregion

		#region PhotonCallbacks
		void OnFailedToConnectToPhoton () {
			if (currentRetries <= 0) {
				currentRetries = maxRetries;
				Debug.Log("Reconnect failed.");
				MessageManager.Send(connectFailedMessage);
			}
			else {
				currentRetries--;
				StartCoroutine(AttemptLogin());
				Debug.Log("Attempting reconnection");

			}
		}

		void OnDisconnectedFromPhoton () {
			MessageManager.Send(connectFailedMessage);
		}

		void OnConnectedToPhoton() {
			if (debug)
				Debug.Log("Connected to Photon.");
			if (!PhotonNetwork.insideLobby)
				PhotonNetwork.JoinLobby();
		}

		private void OnLeftRoom () {
			PhotonNetwork.JoinLobby();
		}

		void OnJoinedLobby() {
			rooms.Clear();
//			rooms.AddRange( PhotonNetwork.GetRoomList());
			if (debug)
				Debug.Log("Entered the lobby, showing " + rooms.Count + " channels");
			interfaceMode = InterfaceModes.Channel;
		}

		#endregion
		#region messages
		public void OpenLoginMenu () {
			useLegacyGUI = true;
			interfaceMode = InterfaceModes.Login;
		}
		public void OpenChannelMenu () {
			useLegacyGUI = true;
			interfaceMode = InterfaceModes.Channel;
		}
		public void CloseChannelMenu () {
			useLegacyGUI = false;
		}
		public void ToggleChannelMenu () {
			useLegacyGUI = !useLegacyGUI;
		}
		public void ChangeScene (string _targetScene ) {
			StartCoroutine(SwapScene(_targetScene));
		}

		public void ChangeChannel (int _newChannel) {
			channel = _newChannel;
			StartCoroutine(SwapScene(targetScene));
		}
		#endregion

	}
}