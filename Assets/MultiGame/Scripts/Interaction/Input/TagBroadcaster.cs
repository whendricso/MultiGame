using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class TagBroadcaster : MultiModule {

		[Tooltip("Tag associated with objects we want to talk to")]
		public string targetTag = "";
		[Tooltip("The message we want to send those objects")]
		public MessageManager.ManagedMessage message;

		[Tooltip("Should we auto-broadcast when we are created?")]
		public bool onStart = false;
		[Tooltip("Should we auto-broadcast every single frame?")]
		public bool onUpdate = false;

		public HelpInfo help = new HelpInfo("This component sends messages to all objects of a given tag. Can activate automatically, or based on other message senders with " +
			"'Broadcast'");

		public bool debug = false;

		void Start () {
			if (onStart) {
				Broadcast();
			}
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref message, gameObject);
		}

		void Update () {
			if (onUpdate) {
				Broadcast();
			}
		}

		public void Broadcast () {
			if (message.target != null) {
				if (debug)
					Debug.Log("Broadcasting " + message.message + " to " + message.target.name);
				MessageManager.SendTo(message, message.target);
			}
			else {
				if (debug)
					Debug.Log("Broadcasting to tag " + targetTag);
				foreach (GameObject obj in GameObject.FindGameObjectsWithTag(targetTag))
					MessageManager.SendTo(message, obj);
			}
		}
	}
}