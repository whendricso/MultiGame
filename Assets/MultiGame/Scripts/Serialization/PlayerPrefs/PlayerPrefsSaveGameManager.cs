using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Serialization/PlayerPrefs Save Game Manager")]
	public class PlayerPrefsSaveGameManager : MultiModule {

		[Tooltip("Normalized viewport rectangle indicating the percentages of screen space where the legacy GUI will appear, if used. Not suitable for mobile devices")]
		public Rect guiArea = new Rect(.01f, .01f, .5f, .5f);
		[RequiredFieldAttribute("Unique window identifier, must be different from all other windows in the game")]
		public int windowID = 7849024;
		[Tooltip("Should we show the legacy Unity GUI? Not suitable for mobile devices")]
		public bool showGui = false;

		public HelpInfo help = new HelpInfo("Player Prefs Save Game Manager allows you to clear Player Prefs");

		void OnGUI () {
			if (!(guiArea.width > 0f) || !showGui)
				return;

			GUILayout.Window(windowID, new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), SaveWindow,"Saved User Preferences");
		}

		private void SaveWindow (int _id) {
			if (GUILayout.Button("Clear all player preferences"))
				ClearPrefs();
		}

		public MessageHelp clearPrefsHelp = new MessageHelp("ClearPrefs","DELETES ALL PLAYER PREFS, CANNOT BE UNDONE!");
		public void ClearPrefs () {
			PlayerPrefs.DeleteAll();
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Open the Legacy Unity GUI");
		public void OpenMenu () {
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Close the Legacy Unity GUI");
		public void CloseMenu () {
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toggle the Legacy Unity GUI");
		public void ToggleMenu () {
			showGui = !showGui;
		}
	}
}