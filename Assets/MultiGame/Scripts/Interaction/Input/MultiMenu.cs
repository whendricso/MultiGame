using UnityEngine;
using System.Collections;

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
		public MessageManager.ManagedMessage message;
	}

	public Button[] buttons;

	void Start () {
		foreach(Button button in buttons) {
			if (button.message.target == null)
				button.message.target = gameObject;
		}
		if (persistent)
			DontDestroyOnLoad(gameObject);
	}
	
	void OnGUI () {
		if (!showMenu)
			return;
		if (guiSkin != null)
			GUI.skin = guiSkin;
		GUILayout.BeginArea(new Rect(screenArea.x * Screen.width, screenArea.y * Screen.height, screenArea.width * Screen.width, screenArea.height * Screen.height),"","box");
		GUILayout.Label(infoText, "label");
		for(int i = 0; i < buttons.Length; i++) {
			if (GUILayout.Button(buttons[i].button)) {
				if (closeOnSelect)
					showMenu = false;
				MessageManager.Send(buttons[i].message);

			}
		}
		GUILayout.EndArea();

	}

	void OpenMenu () {
		if (debug)
			Debug.Log("Open");
		showMenu = true;
	}

	void CloseMenu () {
		if (debug)
			Debug.Log("Close");
		showMenu = false;
	}

	void ToggleMenu() {
		if (debug)
			Debug.Log("Toggle");
		showMenu = !showMenu;
	}
}
