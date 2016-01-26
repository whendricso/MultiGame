using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	public class Clickable : MultiModule {
		
		[Tooltip("What should we send when clicked?")]
		public MessageManager.ManagedMessage message;
		[Tooltip("Do we need to both click, and release while keeping the mouse over the object?")]
		public bool asButton = true;
		public bool debug = false;
		[Tooltip("Does the user need to hold down a key while clicking?")]
		public KeyCode modifier = KeyCode.None;

		public HelpInfo help = new HelpInfo("This component allows a message to be sent when this object's collider is clicked. If you don't see the message you want, click " +
			"'Rescan For Messages' and MultiGame will build a new list.");
		
		void Start () {
			if (message.target == null) {
				message.target = gameObject;
			}
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref message, gameObject);
		}
		
		void OnMouseUpAsButton() {
			if (asButton)
				ClickMessage();
		}

		void OnMouseDown () {
			if (!asButton)
				ClickMessage();
		}

		void ClickMessage () {
			if (modifier != KeyCode.None && !Input.GetKey(modifier)) {
				if (debug)
					Debug.Log("Click discarded");
				return;
			}
			if (debug)
				Debug.Log("Clicked " + gameObject.name);
			MessageManager.Send(message);//target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}

	}
}