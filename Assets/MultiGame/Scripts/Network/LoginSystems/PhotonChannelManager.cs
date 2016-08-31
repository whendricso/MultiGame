using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using MultiGame;

namespace MultiGame {
	public class PhotonChannelManager : Photon.MonoBehaviour {

		#region MemberVariables
		[RequiredFieldAttribute("Which version of the game are we running? Remember to change this so that players with different versions don't connect to each other")]
		public string gameVersion = "0.0";
		[RequiredFieldAttribute("Default name for new users")]
		public string characterName = "New Bee";
		[RequiredFieldAttribute("Default channel name. MultiGame considers each Unity scene to be a zone, only connecting to other players in the same zone, channel name, and channel number. Channel number is set by the " +
			"user, or automatically incremented when a given channel is full in a given area.")]
		public string channelName = "Public";

		[RequiredFieldAttribute("The name of the first scene the player should be taken to when they join the game.")]
		public string firstScene = "";

		[RequiredFieldAttribute("How many times should MultiGame try to reconnect when the connection fails?")]
		public int maxRetries = 3;
		public MessageManager.ManagedMessage connectFailedMessage;
		public MessageManager.ManagedMessage disconnectedMessage;
		private int currentRetries;
		[RequiredFieldAttribute("A unique ID for this window, must be different from any other IMGUI window IDs used in your game.")]
		public int windowID = 934758;
		[System.NonSerialized]
		public int channel = 0;

		[RequiredFieldAttribute("How many players can we have in a channel before we block entry?")]
		public int maxPerChannel = 32;
//		[RequiredFieldAttribute("How many players should a channel have before adding new players elsewhere?")]
//		public int idealPerChannel = 28;

		public enum InterfaceModes {Collapsed, Login, Channel};
		[HideInInspector]
		public InterfaceModes interfaceMode = InterfaceModes.Login;
		[System.NonSerialized]
		public string targetScene;
		[Tooltip("Should we use the IMGUI for this? Not compatible with mobile devices.")]
		public bool useLegacyGUI = true;
		[Tooltip("Optional skin to use with the IMGUI")]
		public GUISkin guiSkin;
		[Tooltip("Color to tint the GUI")]
		public Color guiColor = Color.white;
		[Tooltip("IMGUI window position & size in normalized viewport coordinates. Each number represents a percentage of screen space between 0 and 1")]
		public Rect guiArea = new Rect(.01f,.01f,.2f,.98f);
		[Tooltip("Button rectangle to open the Channel window when collapsed in normalized viewport coordinates. Each number represents a percentage of screen space between 0 and 1")]
		public Rect collapsedGuiArea = new Rect(.01f,.01f,.2f,.025f);

		[System.NonSerialized]
		public string currentStatus = "";

		[System.NonSerialized]
		public List<RoomInfo> rooms = new List<RoomInfo>();
		private Vector2 scrollArea;

		//used for making new channels
		private int newChan = 0;
		private string newChanName = "";

		public bool debug = false;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Channel Manager handles Photon connections and allows players to change scene but remain on the same channe. " +
			"This creates a separate Photon 'room' for players in separate Unity scenes, so they can't use in-room chat, but if players on the same channel are in the same scene they can " +
			"play and chat together, allowing both exploration and play with friends.");

		#endregion

		#region MeatAndPotatoes

		void Awake () {
			DontDestroyOnLoad(this.gameObject);
			targetScene = firstScene;
			currentRetries = maxRetries;
			if (PlayerPrefs.HasKey("channelName")) {
				newChanName = PlayerPrefs.GetString("channelName");
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
			if (guiSkin != null)
				GUI.skin = guiSkin;
			GUI.color = guiColor;

			if (!PhotonNetwork.connected)
				interfaceMode = InterfaceModes.Login;

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
				if ( GUI.Button(new Rect( collapsedGuiArea.x * Screen.width, collapsedGuiArea.y * Screen.height, collapsedGuiArea.width * Screen.width, collapsedGuiArea.height * Screen.height), "Chan"))
					interfaceMode = InterfaceModes.Channel;
				break;
			}

			GUI.color = Color.white;

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
			if (GUILayout.Button("Join Game")) {
				targetScene = firstScene;
				ConnectWithName(characterName);
			}
			GUILayout.EndVertical();
		}

		private void ChannelWindow (int _id) {
			

			scrollArea = GUILayout.BeginScrollView(scrollArea, "box", GUILayout.ExpandHeight(true));

			if (PhotonNetwork.inRoom)
				GUILayout.Label("In: " + PhotonNetwork.room.name);
			if (PhotonNetwork.insideLobby)
				GUILayout.Label("In lobby, showing " + PhotonNetwork.countOfRooms + " rooms & " + PhotonNetwork.otherPlayers.Length + " other players.");
			

			if (PhotonNetwork.insideLobby) {
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
					StartCoroutine(SwapScene(targetScene));
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
				}
			}
			if (PhotonNetwork.inRoom) {
				if (GUILayout.Button("Drop to Lobby", GUILayout.Height(64f)))
					PhotonNetwork.LeaveRoom();

				if (PhotonNetwork.otherPlayers.Length > 0) {
					foreach (PhotonPlayer _player in PhotonNetwork.otherPlayers) {
						GUILayout.BeginHorizontal();
						GUILayout.Label(_player.name);
						if (PhotonNetwork.player.isMasterClient) {
							GUI.color = Color.red;
							if (GUILayout.Button("X"))
								PhotonNetwork.CloseConnection(_player);
							GUI.color = Color.white;
						}
						GUILayout.EndHorizontal();
					}
				} else {
					GUILayout.Label("You are the only one in this channel & location");
				}
			}

			GUILayout.EndScrollView();

			if (GUILayout.Button("Collapse"))
				interfaceMode = InterfaceModes.Collapsed;
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

		//this method should not be accessed directly, use ChangeScene or ChangeChannel instead
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

			PhotonNetwork.isMessageQueueRunning = true;

			if (PhotonNetwork.connectionState == ConnectionState.Connected) {
				if (PhotonNetwork.room == null) {
					RoomOptions options = new RoomOptions();
					options.isOpen = true;
					options.MaxPlayers = (byte)maxPerChannel;
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
				interfaceMode = InterfaceModes.Login;
			}
			else {
				currentRetries--;
				StartCoroutine(AttemptLogin());
				Debug.Log("Attempting reconnection");
			}
		}

		void OnPhotonJoinRoomFailed (object[] _fault) {
			if((int)_fault[0] == 32765) {//room full!
				channel++;
				StartCoroutine(SwapScene(targetScene));
			} else {
				PhotonNetwork.JoinLobby();
			}
		}

		void OnDisconnectedFromPhoton () {
			MessageManager.Send(disconnectedMessage);
			interfaceMode = InterfaceModes.Login;
		}

		void OnConnectedToPhoton() {
			if (debug)
				Debug.Log("Connected to Photon.");
		}

		void OnConnectedToMaster () {
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
			if (debug)
				Debug.Log("Entered room " +  PhotonNetwork.room.name);
			interfaceMode = InterfaceModes.Collapsed;
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
//		public MultiModule.MessageHelp openLoginMenuHelp = new MultiModule.MessageHelp("OpenLoginMenu","Opens a legacy GUI for connecting to the game cloud");
//		public void OpenLoginMenu () {
//			useLegacyGUI = true;
//			interfaceMode = InterfaceModes.Login;
//		}
		public MultiModule.MessageHelp openChannelMenuHelp = new MultiModule.MessageHelp("OpenChannelMenu","Opens a legacy IMGUI for connecting to game channels");
		public void OpenChannelMenu () {
			useLegacyGUI = true;
//			interfaceMode = InterfaceModes.Channel;
		}
		public MultiModule.MessageHelp closeChannelMenuHelp = new MultiModule.MessageHelp("CloseChannelMenu","Closes a legacy IMGUI for connecting to game channels");
		public void CloseChannelMenu () {
			useLegacyGUI = false;
		}
		public MultiModule.MessageHelp toggleChannelMenuHelp = new MultiModule.MessageHelp("ToggleChannelMenu","Toggles a legacy IMGUI for connecting to game channels");
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