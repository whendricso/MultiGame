using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class OnToggleMessage : MultiModule {

		public List<MessageManager.ManagedMessage> enabledMessages = new List<MessageManager.ManagedMessage>();
		public List<MessageManager.ManagedMessage> disabledMessages = new List<MessageManager.ManagedMessage>();

		public bool debug = false;

		void OnValidate () {
			for (int i = 0; i < enabledMessages.Count; i++) {
				MessageManager.ManagedMessage msg = enabledMessages [i];
				MessageManager.UpdateMessageGUI (ref msg, gameObject);
			}
			for (int i = 0; i < disabledMessages.Count; i++) {
				MessageManager.ManagedMessage msg = disabledMessages [i];
				MessageManager.UpdateMessageGUI (ref msg, gameObject);
			}	
		}

		void Start () {
			foreach(MessageManager.ManagedMessage message in enabledMessages) {
				if (message.target == null)
					message.target = gameObject;
			}
			foreach(MessageManager.ManagedMessage message in enabledMessages) {
				if (message.target == null)
					message.target = gameObject;
			}
		}

		void OnEnable () {
			if (debug)
				Debug.Log ("OnToggleMessage " + gameObject.name + " was enabled, sending messages");
			foreach(MessageManager.ManagedMessage message in enabledMessages) {
				MessageManager.Send (message);
			}
		}

		void OnDisable () {
			if (debug)
				Debug.Log ("OnToggleMessage " + gameObject.name + " was disabled, sending messages");
			foreach(MessageManager.ManagedMessage message in disabledMessages) {
				MessageManager.Send (message);
			}
		}
	}
}