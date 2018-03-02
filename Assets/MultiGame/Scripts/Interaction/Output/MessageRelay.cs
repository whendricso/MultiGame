using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageRelay")]
	public class MessageRelay : MultiModule {

		[ReorderableAttribute]
		[Header("Important - Must be Populated")]
		[Tooltip("List of messages we will send")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		[Tooltip("Should we ignore relay calls when this object or component is disabled? If false, we will relay even when disabled!")]
		public bool ignoreWhenDisabled = true;

		public HelpInfo help = new HelpInfo("This component sends a list of messages when receiving the 'Relay' message. You can attach this to a child object (appropriately named)" +
			" and use it as a logic gate, or just expand the reach of other message senders based on context." +
			"\n\n" +
			"To use, populate the list of 'Messages' above with what ever you'd like to send to other objects or components. Then attach a Message sender of some kind to another object, possibly this object's parent. " +
			"When this component receives 'Relay' it will send all of the messages in the list. To write a message in a script, simply implement a new method as such:" +
			"\n" +
			"public void MyNewMessage( )\n" +
			"The message you implement may have 0 arguments, or it may take an integer, float, bool, or string. This will tell MultiGame to add it to the Messages list when you hit 'Refresh Messages'. Any method that fits the argument " +
			"types may be called, even if it does not appear, by locking the message sender and then typing in the method name manually. When you do this, MultiGame will turn the message box cyan to indicate that you're sending " +
			"a custom message.");

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

		public MessageHelp relayHelp = new MessageHelp("Relay","Activates this Message Relay, sending all 'Messages'");
		public void Relay () {
			if (ignoreWhenDisabled) {
				if (enabled == false || gameObject.activeSelf == false)
					return;
			}
			if (debug)
				Debug.Log ("Message Relay " + gameObject.name + " activated, sending " + messages.Count + " messages.");
			foreach(MessageManager.ManagedMessage msg in messages) {
				if (debug)
					Debug.Log ("Message relay " + gameObject.name + " is sending " + msg.message + " to target " + msg.target);
				MessageManager.Send(msg);
			}
		}
	}
}