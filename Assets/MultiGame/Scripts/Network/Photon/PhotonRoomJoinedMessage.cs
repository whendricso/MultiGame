﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Room Joined Message")]
	public class PhotonRoomJoinedMessage : PhotonModule {

		public MessageManager.ManagedMessage message;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Room Joined Message sends a message when this object joins a Photon room. Useful to cause some logic to happen when a new player joins");

		public bool debug = false;

		void Start () {
			if (message.target == null)
				message.target = gameObject;
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref message, gameObject);
		}

		public override void OnJoinedRoom () {
			if (debug)
				Debug.Log("Photon Room Joined Message " + gameObject.name + " is sending the message " + message.message);
			MessageManager.Send(message);
		}
	}
}