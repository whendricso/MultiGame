using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageRelay")]
	public class MessageRelay : MultiModule {

		[Tooltip("List of messages we will send")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		public HelpInfo help = new HelpInfo("This component sends a list of messages when receiving the 'Relay' message. You can attach this to a child object (appropriately named)" +
			" and use it as a logic gate, or just expand the reach of other message senders based on context.");

		public bool debug = false;

		void Start () {
			if (messages.Count < 1) {
				Debug.LogError("Message Relay " + gameObject.name + " needs a list of messages!");
				enabled = false;
				return;
			}

			foreach(MessageManager.ManagedMessage msg in messages) {
				if (msg.target == null)
					msg.target = gameObject;
			}
		}

		void OnValidate () {
			for (int i = 0; i < messages.Count; i++) {
				MessageManager.ManagedMessage _msg = messages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}
		
		public void Relay () {
			if (enabled == false)
				return;

			foreach(MessageManager.ManagedMessage msg in messages) {
				if (debug)
					Debug.Log ("Message relay " + gameObject.name + " is sending " + msg.message);
				MessageManager.Send(msg);
			}
		}
	}
}