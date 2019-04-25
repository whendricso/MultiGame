using UnityEngine;
using System.Collections;
using MultiGame;
//using System.IO;

namespace MultiGame {

	public class CustomInfo : MultiModule {

		[Tooltip("Should we show the GUI by default?'OpenMenu' 'CloseMenu' and 'ToggleMenu' messages can be used to control this. Note: for mobile use Unity's new UI instead. ")]
		public bool showGui = false;
		[Tooltip("What skin, if any, should we put on this info box?")]
		public GUISkin guiSkin;
		[Tooltip("Normalized viewport rectangle indicating the area where the GUI will appear. Values must be between 0 and 1 and indicate a percentage of screen space")]
		public Rect guiArea = new Rect(.3f,.3f,.3f,.3f);

		[Tooltip("Can the player edit this information box?")]
		public bool editable = true;
		private bool editing = false;

//		public string fileName = "";
		[Tooltip("The unique name of this object. This is the key that will be used to store it's information text.")]
		public string customName = "";
		[Tooltip("Default text, if any, that should appear prior to user editing.")]
		public string infoText = "";

//		private MemoryStream stream;
//		private StringWriter writer;
//		private StringReader reader;

		private Vector2 scrollArea = Vector2.zero;

		public HelpInfo help = new HelpInfo("Cusom Info allows the developer or player to create descriptions or notes, which are saved in PlayerPrefs based on their name if changed by the player.");

		void OnEnable () {
			Load();
		}

		void OnGUI () {
			GUILayout.BeginArea(new Rect(Screen.width * guiArea.x, Screen.height * guiArea.y, Screen.width * guiArea.width, Screen.height * guiArea.height), customName, "box");
			scrollArea = GUILayout.BeginScrollView(scrollArea);
			GUILayout.FlexibleSpace();
			if (!editing)
				GUILayout.Label(infoText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			else {
//				GUILayout.Label("File Name:");
//				fileName = GUILayout.TextField(fileName);
				GUILayout.Label("Custom Name (shown in title):");
				customName = GUILayout.TextField(customName);
				GUILayout.Label("Info:");
				infoText = GUILayout.TextArea(infoText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			}
			GUILayout.FlexibleSpace();

			if (editable) {
				if (editing) {
					if (GUILayout.Button("Okay")) {
						editing = false;
						Save();
					}
				} else {
					if (GUILayout.Button("Edit"))
						editing = true;
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		public void Save () {
			if (!string.IsNullOrEmpty(customName) && !string.IsNullOrEmpty(infoText)) {
				PlayerPrefs.SetString(customName, infoText);
				PlayerPrefs.Save();
			}
		}

		public void Load () {
			if (PlayerPrefs.HasKey(customName))
				infoText = PlayerPrefs.GetString(customName);
		}

		public void OpenMenu() {
			showGui = true;
		}

		public void CloseMenu() {
			showGui = false;
		}

		public void ToggleMenu() {
			showGui = !showGui;
		}
	}
}










































//MultiGame copyright 2012-2016 William Hendrickson all rights reserved