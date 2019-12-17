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
		[Tooltip("If true, the menu will be displayed as a window. If false, it will be displayed in a layout box instead (to conserve screen space)")]
		public bool asWindow = false;
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
		[Tooltip("Would you like to display an image, like an illustration or character portrait?")]
		public Texture2D image;
		[Tooltip("What is the maximum number of pixels we should take up with the image? If undefined, the image size itself will be used.")]
		public Vector2 maxImageSize = Vector2.zero;
		[Reorderable]
		public Button[] buttons;
		public bool debug = false;

		public HelpInfo help = new HelpInfo("This is a generic implementation of Unity's built-in GUI. Not suitable for mobile devices. " +
			"To use, add this component to any object, assign some text, buttons, and perhaps an image. For each button, assign some messages for " +
			"it to send to other components, to implement the desired selection into your game. You can also combine many MultiMenu components " +
			"together using the MultiGame Toolbar to add each one, creating dialogue trees and multi-tiered menus.");

		[System.Serializable]
		public class Button {
			public string button;
			[HideInInspector]
			public MessageManager.ManagedMessage message;
			public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();
		}

		void Awake () {
			foreach (Button button in buttons) {
				if (button.message != null && !button.messages.Contains(button.message))
					button.messages.Add(button.message);
			}
		}

		void OnEnable () {
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
			if (buttons == null)
				return;
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
			GUILayout.BeginArea(new Rect(screenArea.x * Screen.width, screenArea.y * Screen.height, screenArea.width * Screen.width, screenArea.height * Screen.height),"",(asWindow ? "window" : "box"));
			if (image != null)
				GUILayout.Box(image, GUILayout.MaxWidth(maxImageSize.x > 0 ? maxImageSize.x : image.width), GUILayout.MaxHeight(maxImageSize.y > 0 ? maxImageSize.y : image.height));
			GUILayout.FlexibleSpace();
			if (!string.IsNullOrEmpty(infoText))
				GUILayout.Label(infoText, "label");

			GUILayout.FlexibleSpace();

			for(int i = 0; i < buttons.Length; i++) {
				if (GUILayout.Button(buttons[i].button)) {
					if (closeOnSelect)
						showGui = false;
					foreach (MessageManager.ManagedMessage msg in buttons[i].messages)
						MessageManager.Send(msg);

				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Opens the IMGUI");
		public void OpenMenu () {
			if (!gameObject.activeInHierarchy)
				return;
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
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Toggle");
			showGui = !showGui;
		}
	}
}