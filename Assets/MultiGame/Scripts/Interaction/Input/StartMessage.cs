﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/StartMessage")]
public class StartMessage : MultiModule {

		[Tooltip("When this object is created, what message should we send?")]
		public MessageManager.ManagedMessage[] messages;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("This component sends messages as soon as the object is created.");

		void Start () {
			foreach (MessageManager.ManagedMessage _message in messages) {
				if (debug)
					Debug.Log("Start Message " + gameObject.name + " sent " + _message.message);
				if (_message.target == null)
					_message.target = gameObject;
				MessageManager.Send(_message);
			}
		}

		void OnValidate () {
			for (int i = 0; i < this.messages.Length; i++) {
				MessageManager.ManagedMessage _msg = messages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}
	}
}