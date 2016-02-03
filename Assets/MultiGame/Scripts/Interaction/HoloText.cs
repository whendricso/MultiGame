using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Holo Text")]
	public class HoloText : MultiModule {

		[Tooltip("Reference to the text mesh object we're manipulating")]
		public TextMesh textMesh;
		[Tooltip("Normalized viewport rectangle indicating the area for the optional legacy Unity GUI for editing the text")]
		public Rect guiArea = new Rect(.3f, .3f, .3f, .3f);
		public GUISkin guiSkin;
		[Tooltip("Unique identifier for this GUI window")]
		public int windowID = 875439;
		[Tooltip("A unique string which identifies this particular custom text object")]
		public string uniqueTextKey;

		[Tooltip("Should we use a legacy Unity GUI?")]
		private bool showWindow = false;

		public HelpInfo help = new HelpInfo("This component allows the user to change the text of a 3D text object. This can be a fun way to add user-defined labels to your game." +
			"\n----Messages:----\n" +
			"'ToggleTextWindow' takes no parameters and turns the text editor GUI on/off\n" +
			"'Save' and 'Load' take no parameters and serialize the text value to/from Player Prefs" +
			"'Change Text' takes a string and changes the text to what ever you like");

		void Reset () {
			if (string.IsNullOrEmpty(uniqueTextKey))
				uniqueTextKey = Random.value.ToString();
		}

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
			if (guiSkin != null)
				GUI.skin = guiSkin;
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

		public void Save () {
			PlayerPrefs.SetString(uniqueTextKey, textMesh.text);
			PlayerPrefs.Save();
		}

		public void Load () {
			if (PlayerPrefs.HasKey(this.uniqueTextKey)) {
				textMesh.text = PlayerPrefs.GetString(uniqueTextKey);
			}
		}

	}
}