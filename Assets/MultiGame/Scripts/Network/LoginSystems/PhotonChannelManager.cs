using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using MultiGame;

namespace MultiGame {
	public class PhotonChannelManager : Photon.MonoBehaviour {

		#region MemberVariables
		public string gameVersion = "0.0";
		public string characterName = "New Bee";
		public string channelName = "";

		[Tooltip("The name of the first scene the player should be taken to when they join the game.")]
		public string firstScene = "";

		public int maxRetries = 3;
		public MessageManager.ManagedMessage connectFailedMessage;
		public MessageManager.ManagedMessage disconnectedMessage;
		private int currentRetries;
		public int windowID = 934758;
		[System.NonSerialized]
		public int channel = 0;

		[RequiredFieldAttribute("How many players can we have in a channel before we block entry?")]
		public int maxPerChannel = 32;
		[RequiredFieldAttribute("How many players should a channel have before adding new players elsewhere?")]
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

		//used for making new channels
		private int newChan = 0;
		private string newChanName = "";

		public bool debug = false;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Channel Manager allows players to change scene but remain on the same channel, if you desire. This creates a separate Photon 'room' for players in separate Unity " +
			"scenes, so they can't use in-room chat, but if players on the same channel are in the same scene they can play together, allowing both exploration and play with friends. This system also controls for " +
			"server population changes, adding a new channel when one becomes near-full. It attempts to never fill the channel all the way by default, adjust this by changing the 'Ideal Per Channel' count.");

		#endregion

		#region MeatAndPotatoes

		void Awake () {
			DontDestroyOnLoad(this.gameObject);
			targetScene = firstScene;
			currentRetries = maxRetries;
			if (PlayerPrefs.HasKey("channelName")) {
				newChanName =PlayerPrefs.GetString("channelName");
			} else {
			
				if (!string.IsNullOrEmpty(channelName))
					newChanName = channelName;
				else
					newChanName = Random.Range(0,10000).ToString();
			}
			if (PlayerPrefs.HasKey("characterName"))
				characterName = PlayerPrefs.GetString("characterName");

			PhotonChannelManager[] _managers = FindObjectsOfType<PhotonChannelManager>();
			if (_managers.Length > 1) {
				Debug.LogError("Photon Channel Manager " + gameObject.name + " has detected that there is more than one in the scene. Please make sure there is always exactly one Photon Channel Manager, " +
					"added in one of the first scenes in your game. It will persist between scenes until destroyed.");
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
				if ( GUI.Button(new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), "Channels"))
					interfaceMode = InterfaceModes.Channel;
				break;
			}
		}

//		private void CollapsedWindow (int _id) {
//			GUILayout.BeginVertical("box");
//			if (GUILayout.Button("Drop to Lobby"))
//				PhotonNetwork.LeaveRoom();
//			GUILayout.EndVertical();
//
//		}

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
			GUILayout.Space(16f);

			GUILayout.Label("New Channel Settings:");
			GUILayout.Label("Channel Name:");
			newChanName = GUILayout.TextField(newChanName, GUILayout.ExpandWidth(true));
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("+"))
				newChan ++;
			if(GUILayout.Button(""+newChan))
				newChan = 0;
			if(GUILayout.Button("-"))
				newChan--;
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Create Channel", GUILayout.Height(64f))) {
				channel = newChan;
				channelName = newChanName;
				PlayerPrefs.SetString("channelName", channelName);
				PlayerPrefs.Save();
				StartCoroutine(SwapScene(SceneManagerHelper.ActiveSceneName));
			}

			if (rooms.Count > 0) {
				GUILayout.Label("Select a channel to join:");
				foreach (RoomInfo _rinfo in rooms) {
					GUILayout.BeginHorizontal("box", GUILayout.Height(64f));
					if (_rinfo.name.Contains(SceneManagerHelper.ActiveSceneName)) {
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
					PhotonNetwork.JoinOrCreateRoom(GetChannelString(), options, TypedLobby.Default);
				}

			}

			GUILayout.EndScrollView();
		}

		public MultiModule.MessageHelp connectHelp = new MultiModule.MessageHelp("Connect","Connects to the Photon Cloud with the current 'Character Name'");
		public void Connect() {
			ConnectWithName(characterName);
		}

		public MultiModule.MessageHelp connectWithNameHelp = new MultiModule.MessageHelp("ConnectWithName","Connects to the Photon Cloud and allows you to supply a specific name",4,"The player name we want to join with");
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
			targetScene = SceneManagerHelper.ActiveSceneName;
			
			if (PhotonNetwork.connectionState == ConnectionState.Connected) {
				if (PhotonNetwork.room == null) {
					RoomOptions options = new RoomOptions();
					options.isOpen = true;
					options.isVisible = true;//TODO: implement private channels
					PhotonNetwork.JoinOrCreateRoom(GetChannelString(), options, TypedLobby.Default);
				}
			}
			else {
				Debug.Log("But we were not connected!");
			}
		}
		private string GetChannelString () {
			return targetScene + channelName + channel;
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

		void OnPhotonJoinRoomFailed () {
			PhotonNetwork.JoinLobby();
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
			
		void OnJoinedRoom () {
			rooms.Clear();
			newChan = channel;
			newChanName = channelName;
		}

		void OnJoinedLobby() {
			rooms.Clear();
			rooms.AddRange( PhotonNetwork.GetRoomList());
			if (debug)
				Debug.Log("Entered the lobby, showing " + rooms.Count + " channels");
			interfaceMode = InterfaceModes.Channel;
		}

		#endregion
		#region messages
		public MultiModule.MessageHelp openLoginMenuHelp = new MultiModule.MessageHelp("OpenLoginMenu","Opens a legacy GUI for connecting to the game cloud");
		public void OpenLoginMenu () {
			useLegacyGUI = true;
			interfaceMode = InterfaceModes.Login;
		}
		public MultiModule.MessageHelp openChannelMenuHelp = new MultiModule.MessageHelp("OpenChannelMenu","Opens a legacy GUI for connecting to game channels");
		public void OpenChannelMenu () {
			useLegacyGUI = true;
			interfaceMode = InterfaceModes.Channel;
		}
		public MultiModule.MessageHelp closeChannelMenuHelp = new MultiModule.MessageHelp("CloseChannelMenu","Closes a legacy GUI for connecting to game channels");
		public void CloseChannelMenu () {
			useLegacyGUI = false;
		}
		public MultiModule.MessageHelp toggleChannelMenuHelp = new MultiModule.MessageHelp("ToggleChannelMenu","Toggles a legacy GUI for connecting to game channels");
		public void ToggleChannelMenu () {
			useLegacyGUI = !useLegacyGUI;
		}
		public MultiModule.MessageHelp changeSceneHelp = new MultiModule.MessageHelp("ChangeScene","Allows you to go to a different game scene while remaining on this channel", 4, "Name of the scene we want to go to");
		public void ChangeScene (string _targetScene ) {
			StartCoroutine(SwapScene(_targetScene));
		}

		public MultiModule.MessageHelp changeChannelHelp = new MultiModule.MessageHelp("ChangeChannel","Allows you to go to a different channel without changing the scene", 2, "A number for the channel we want to join");
		public void ChangeChannel (string _newChannel) {
			channelName = _newChannel;
			StartCoroutine(SwapScene(targetScene));
		}
		#endregion

	}
}