using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	//MultiMenu Copyright 2013 William Hendrickson and Tech Drone

	[AddComponentMenu("MultiGame/Interaction/Input/MultiMenu")]
	public class MultiMenu : MultiModule {

		//public GameObject target;
		[Tooltip("Should this menu stay around even in between scenes?")]
		public bool persistent = false;
		public GUISkin guiSkin;
		[Tooltip("Normalized viewport rectangle designating the screen area for the menu, values between 0 and 1")]
		public Rect screenArea = new Rect(.3f,.3f,.3f,.3f);
		[Tooltip("Should the menu be currently visible?")]
		public bool showGui = false;
		[Tooltip("Should the menu close as soon as you click something?")]
		public bool closeOnSelect = true;
		[Tooltip("What sort of text prompt do we need?")]
		[Multiline()]
		public string infoText;
		public bool debug = false;

		public HelpInfo help = new HelpInfo("This is a generic implementation of Unity's built-in GUI. Not suitable for mobile devices. " +
			"By combining many of these together in a tree in the Heirarchy and setting targets for each message manually, you can create dialogue trees. " +
			"You can also use this component to implement pause menus, save/load menus, graphics settings menus for toggling postprocess effects, and really anything you like, " +
			"as long as what you like is based on text and buttons.");

		[System.Serializable]
		public class Button {
			public string button;
			[HideInInspector]
			public MessageManager.ManagedMessage message;
			public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();
		}

		public Button[] buttons;

		void Awake () {
			foreach (Button button in buttons) {
				if (button.message != null && !button.messages.Contains(button.message))
					button.messages.Add(button.message);
			}
		}

		void Start () {
			foreach(Button button in buttons) {
				foreach(MessageManager.ManagedMessage msg in button.messages) {
					if (msg.target == null)
						msg.target = gameObject;
				}
			}
			if (persistent)
				DontDestroyOnLoad(gameObject);
		}

		void OnValidate () {
			for (int i = 0; i < buttons.Length; i++) {
				for (int j = 0; j < buttons[i].messages.Count; j++) {
					MessageManager.ManagedMessage _msg = buttons[i].messages[j];
					MessageManager.UpdateMessageGUI(ref _msg, gameObject);
				}
			}
		}
		
		void OnGUI () {
			if (!showGui)
				return;
			if (guiSkin != null)
				GUI.skin = guiSkin;
			GUILayout.BeginArea(new Rect(screenArea.x * Screen.width, screenArea.y * Screen.height, screenArea.width * Screen.width, screenArea.height * Screen.height),"","box");
			if(!string.IsNullOrEmpty(infoText))
			GUILayout.Label(infoText, "label");
			for(int i = 0; i < buttons.Length; i++) {
				if (GUILayout.Button(buttons[i].button)) {
					if (closeOnSelect)
						showGui = false;
					foreach (MessageManager.ManagedMessage msg in buttons[i].messages)
						MessageManager.Send(msg);

				}
			}
			GUILayout.EndArea();

		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Opens the IMGUI");
		public void OpenMenu () {
			if (debug)
				Debug.Log("Open");
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Closes the IMGUI");
		public void CloseMenu () {
			if (debug)
				Debug.Log("Close");
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toggles the IMGUI");
		public void ToggleMenu() {
			if (debug)
				Debug.Log("Toggle");
			showGui = !showGui;
		}
	}
}