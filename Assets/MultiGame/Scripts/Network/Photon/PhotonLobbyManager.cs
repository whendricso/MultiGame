using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MultiGame;

namespace MultiGame {

	public class PhotonLobbyManager : PhotonModule {

		public Rect GUIArea = new Rect(.01f,.01f, .98f, .98f);
		public GUISkin guiSkin;
		public bool showGUI = false;

		public string onlineScene = "";

		[System.NonSerialized]
		public Vector2 scrollArea = Vector2.zero;

		private List<RoomInfo> rooms = new List<RoomInfo>();

		void OnGUI () {
			if (!showGUI)
				return;

			GUILayout.BeginArea(new Rect(Screen.width * GUIArea.x, Screen.height * GUIArea.y, Screen.width * GUIArea.width, Screen.height * GUIArea.height),"","box");

			GUILayout.Label("Server List");
			if (GUILayout.Button("Refresh"))
				Refresh();
			scrollArea = GUILayout.BeginScrollView(scrollArea,"box");

			for (int i = 0; i < rooms.Count; i++) {
				if (GUILayout.Button(rooms[i].Name + " : ( " + rooms[i].PlayerCount + "/" + rooms[i].MaxPlayers + " )", GUILayout.ExpandWidth(true)) && (rooms[i].PlayerCount < rooms[i].MaxPlayers)) {
					SceneManager.LoadScene(onlineScene);
					ConnectAfterLoading(rooms[i].name);
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		private void ConnectAfterLoading (string roomName) {
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