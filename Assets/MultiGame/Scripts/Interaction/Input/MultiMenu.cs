using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//MultiMenu Copyright 2013 William Hendrickson and Tech Drone

public class MultiMenu : MonoBehaviour {

	//public GameObject target;
	public bool persistent = false;
	public GUISkin guiSkin;
	public Rect screenArea = new Rect();
	public bool showMenu = false;
	public bool closeOnSelect = true;
	public string infoText;
	public bool debug = false;

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
		if (!showMenu)
			return;
		if (guiSkin != null)
			GUI.skin = guiSkin;
		GUILayout.BeginArea(new Rect(screenArea.x * Screen.width, screenArea.y * Screen.height, screenArea.width * Screen.width, screenArea.height * Screen.height),"","box");
		if(!string.IsNullOrEmpty(infoText))
		GUILayout.Label(infoText, "label");
		for(int i = 0; i < buttons.Length; i++) {
			if (GUILayout.Button(buttons[i].button)) {
				if (closeOnSelect)
					showMenu = false;
				foreach (MessageManager.ManagedMessage msg in buttons[i].messages)
					MessageManager.Send(msg);

			}
		}
		GUILayout.EndArea();

	}

	public void OpenMenu () {
		if (debug)
			Debug.Log("Open");
		showMenu = true;
	}

	public void CloseMenu () {
		if (debug)
			Debug.Log("Close");
		showMenu = false;
	}

	public void ToggleMenu() {
		if (debug)
			Debug.Log("Toggle");
		showMenu = !showMenu;
	}
}
