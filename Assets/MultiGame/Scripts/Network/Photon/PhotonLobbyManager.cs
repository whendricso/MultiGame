using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MultiGame;

namespace MultiGame {

	public class PhotonLobbyManager : PhotonModule {

		public Rect GUIArea = new Rect(.01f,.01f, .98f, .98f);
		public int maxPlayers = 12;
		public GUISkin guiSkin;
		public bool showGUI = false;
		public bool creatingServer = false;
		public string serverName = "The Room";//YOU'RE TEARING ME APART, LISA!
		[MultiGame.RequiredField("The scene which will be used as a game lobby")]
		public string onlineScene = "";

		[System.NonSerialized]
		public Vector2 scrollArea = Vector2.zero;

		private List<RoomInfo> rooms = new List<RoomInfo>();

		private void Start() {
			DontDestroyOnLoad(gameObject);
			StartCoroutine(FirstRefresh());
		}

		IEnumerator FirstRefresh() {
			yield return new WaitForSeconds(.5f);
			Refresh();
		}

		void OnGUI () {
			if (!showGUI)
				return;
			
			if (PhotonNetwork.room == null) {
				GUILayout.BeginArea(new Rect(Screen.width * GUIArea.x, Screen.height * GUIArea.y, Screen.width * GUIArea.width, Screen.height * GUIArea.height), "", "box");
				if (!PhotonNetwork.insideLobby) {
					GUILayout.Label("Joining Lobby...");
				} else {
					if (GUILayout.Button("Create a new game")) {
						creatingServer = true;
					}

					if (!creatingServer) {

						GUILayout.Label("Server List");
						if (GUILayout.Button("Refresh"))
							Refresh();
						scrollArea = GUILayout.BeginScrollView(scrollArea, "box");

						for (int i = 0; i < rooms.Count; i++) {
							if (GUILayout.Button(rooms[i].CustomProperties["serverName"] + " : ( " + rooms[i].PlayerCount + "/" + rooms[i].MaxPlayers + " )", GUILayout.ExpandWidth(true)) && (rooms[i].PlayerCount < rooms[i].MaxPlayers)) {
								StartCoroutine(GoToGame(i));
							}
						}

						GUILayout.EndScrollView();
					} else {
						GUILayout.Label("Server Name");
						serverName = GUILayout.TextField(serverName);
						if (GUILayout.Button("Start Server"))
							CreateGame();
					}
				}
				GUILayout.EndArea();
			}
		}

		public void CreateGame() {
			bool _roomCreated = false;
			RoomOptions _room = new RoomOptions();
			_room = new RoomOptions();
			_room.CleanupCacheOnLeave = false;
			_room.EmptyRoomTtl = 1000;
			_room.IsOpen = true;
			_room.IsVisible = true;
			_room.PublishUserId = true;
			_room.MaxPlayers = (byte)maxPlayers;
			_room.CustomRoomProperties.Add("serverName", serverName);
			_roomCreated = PhotonNetwork.CreateRoom(serverName + onlineScene, _room, new TypedLobby());
			if (!_roomCreated)
				PhotonNetwork.JoinRoom(serverName + onlineScene);
		}

		IEnumerator GoToGame(string _roomName) {
			Debug.Log("Loading and connecting");
			yield return SceneManager.LoadSceneAsync(onlineScene);
			ConnectAfterLoading(_roomName);
		}

		IEnumerator GoToGame(int _index) {
			Debug.Log("Loading and connecting");
			yield return SceneManager.LoadSceneAsync(onlineScene);
			ConnectAfterLoading(rooms[_index].Name);
		}

		public override void OnCreatedRoom() {
			SceneManager.LoadScene(onlineScene);
			base.OnCreatedRoom();
		}

		private void ConnectAfterLoading (string roomName) {
			Debug.Log("Joining room " + roomName);
			PhotonNetwork.JoinRoom(roomName);
		}

		public void Refresh () {
			rooms.Clear();
			rooms.AddRange(PhotonNetwork.GetRoomList());
		}

		public void OpenMenu () {
			showGUI = true;
		}

		public void CloseMenu () {
			showGUI = false;
		}
	
	}
}