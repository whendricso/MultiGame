using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Tag Broadcaster")]
	public class TagBroadcaster : MultiModule {

		[Header("Important - Must be Populated")]
		[Tooltip("List of messages we will send")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();
		[Header("Sender Settings")]
		[RequiredFieldAttribute("Tag associated with objects we want to talk to")]
		public string targetTag = "";


		[Tooltip("Should we auto-broadcast when we are created?")]
		public bool onStart = false;
		[Tooltip("Should we auto-broadcast every single frame? May impact performance, use with discretion!")]
		public bool onUpdate = false;



		public HelpInfo help = new HelpInfo("This component sends messages to all objects of a given tag. Can activate automatically, or based on other message senders with " +
			"'Broadcast'. To use, add at least one Message to the 'Messages' list above, and tag all target objects with an appropriate tag. Then, add that 'Target Tag' in the appropriate field above." +
			"\n\n" +
			"Since the Message list won't likely show what you want when you click 'Refresh Messages' just lock each message and type it in manually. You can find the available messages listed at the bottom of each " +
			"MultiGame component. For non-MultiGame components, you can find messages by opening their source code. And function (even private ones!) with either no argument, or a single bool, int, float, or string argument may " +
			"be used as a Message. Just make sure to type in the name exactly with no typos, and that it's capitalized correctly (Messages are case-sensitive!)");

		public bool debug = false;

		void Start () {
			if (onStart) {
				Broadcast();
			}
		}

		void OnValidate () {
			for (int i = 0; i < messages.Count; i++) {
				MessageManager.ManagedMessage _msg = messages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}

		void Update () {
			if (onUpdate) {
				Broadcast();
			}
		}

		public MessageHelp broadcastHelp = new MessageHelp("Broadcast","Send the 'Messages' to all objects in the scene with 'Target Tag'");
		public void Broadcast () {
			if (debug)
				Debug.Log ("Broadcasting to tag " + targetTag);
			GameObject[] _targets = GameObject.FindGameObjectsWithTag (targetTag);
			foreach (MessageManager.ManagedMessage message in messages) {
				foreach (GameObject obj in _targets)
					MessageManager.SendTo (message, obj);
			}
		}
	}
}