using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiGame;

//TODO: Finish this!
namespace MultiGame {
	public class PhotonConnectionManager : MonoBehaviour {
		public bool connectOnStart = true;

		public bool useGUI = false;
		public Rect guiArea = new Rect(.3f,.3f,.3f,.3f);
		public Color affirmationColor = XKCDColors.LightGrassGreen;
		public Color cancelationColor = XKCDColors.Lavender;
		private bool showGUI = false;

		public MessageManager.ManagedMessage onConnectedMessage;

		private string userNotification = "";

		private void OnValidate() {
			MessageManager.UpdateMessageGUI(ref onConnectedMessage, gameObject);
		}

		private void Start() {
			if (connectOnStart)
				Connect();
			if (onConnectedMessage.target == null)
				onConnectedMessage.target = gameObject;
		}

		private void OnGUI() {
			if (!useGUI)
				return;
			if (!showGUI)
				return;
			GUILayout.BeginArea(guiArea);
			GUILayout.Label(userNotification);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Okay")) {
				userNotification = "";
				showGUI = false;
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Cancel")) {
				userNotification = "";
				showGUI = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		public void Connect() {
			PhotonNetwork.ConnectUsingSettings(PhotonNetwork.gameVersion);
		}

		public void OnConnectedToPhoton() {
			Debug.Log("Connected to Photon ");
			MessageManager.Send(onConnectedMessage);
		}

		public void OnDisconnectedFromPhoton() {
			showGUI = true;
			userNotification = "An error ocurred, and your connection to the server was lost.";
		}

		public void Disconnect() {
			PhotonNetwork.Disconnect();
		}


	}
}