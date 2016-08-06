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
		[RequiredFieldAttribute("A unique string which identifies this particular custom text object", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string uniqueTextKey;

		[Tooltip("Should we use a legacy Unity GUI?")]
		private bool showWindow = false;

		public HelpInfo help = new HelpInfo("This component allows the user to change the text of a 3D text object. This can be a fun way to add user-defined labels to your game.");

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

		public MessageHelp changeTextHelp = new MessageHelp("ChangeText", "Changes the text to what ever you like", 4, "The new text you would like to display");
		public void ChangeText (string _newText) {
			textMesh.text = _newText;
		}

		public MessageHelp saveHelp = new MessageHelp("Save", "Store the text value in Player Prefs under 'Unique Text Key', which indicates the file the string is saved in");
		public void Save () {
			if (string.IsNullOrEmpty(uniqueTextKey) ) {
				Debug.LogError("Holo Text " + gameObject.name + " tried to save without a 'Unique Text Key', please assign one in the inspector!");
				return;
			}
			PlayerPrefs.SetString(uniqueTextKey, textMesh.text);
			PlayerPrefs.Save();
		}

		public MessageHelp loadHelp = new MessageHelp("Load", "Load the text value from Player Prefs using 'Unique Text Key', which uniquely identifies the text file for this object");
		public void Load () {
			if (string.IsNullOrEmpty(uniqueTextKey)) {
						Debug.LogError("Holo Text " + gameObject.name + " tried to load without a 'Unique Text Key', please assign one in the inspector!");
				return;
			}
			if (PlayerPrefs.HasKey(this.uniqueTextKey)) {
				textMesh.text = PlayerPrefs.GetString(uniqueTextKey);
			}
		}

	}
}