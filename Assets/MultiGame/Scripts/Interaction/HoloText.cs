using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class HoloText : MultiModule {

		[Tooltip("Reference to the text mesh object we're manipulating")]
		public TextMesh textMesh;
		[Tooltip("Normalized viewport rectangle indicating the area for the optional legacy Unity GUI for editing the text")]
		public Rect guiArea = new Rect(.3f, .3f, .3f, .3f);
		[Tooltip("Unique identifier for this GUI window")]
		public int windowID = 875439;

		[Tooltip("Should we use a legacy Unity GUI?")]
		private bool showWindow = false;

		public HelpInfo help = new HelpInfo("This component allows the user to change the text of a 3D text object. This can be a fun way to add user-defined labels to your game.");

		void Start () {
			if (textMesh == null)
				textMesh = GetComponentInChildren<TextMesh>();
			if (textMesh == null && transform.parent != null)
				transform.parent.gameObject.GetComponentInChildren<TextMesh>();
			if (textMesh == null) {
				Debug.LogError("HoloText " + gameObject.name + " needs a TextMesh attached!");
				enabled = false;
				return;
			}
		}

		void OnGUI () {
			if(!showWindow)
				return;
			GUILayout.Window(windowID,new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),TextWindow, "Text");
		}

		void TextWindow (int id) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("Text:");
			textMesh.text = GUILayout.TextField(textMesh.text, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			if(GUILayout.Button("Done"))
				showWindow = false;
		}

		public void ToggleTextWindow () {
			showWindow = !showWindow;

		}

		public void ChangeText (string _newText) {
			textMesh.text = _newText;
		}
	}
}