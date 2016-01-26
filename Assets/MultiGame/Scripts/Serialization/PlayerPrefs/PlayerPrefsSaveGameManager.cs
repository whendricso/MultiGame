using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class PlayerPrefsSaveGameManager : MonoBehaviour {

		public Rect guiArea = new Rect();
		public int windowID = 7849024;
		public bool showGui = false;

		void OnGUI () {
			if (!(guiArea.width > 0f) || !showGui)
				return;

			GUILayout.Window(windowID, guiArea, SaveWindow,"Saved User Preferences");
		}

		private void SaveWindow (int _id) {
			if (GUILayout.Button("Clear all player preferences"))
				ClearPrefs();
		}

		public void ClearPrefs () {
			PlayerPrefs.DeleteAll();
		}

		public void OpenMenu () {
			showGui = true;
		}

		public void CloseMenu () {
			showGui = false;
		}

		public void ToggleMenu () {
			showGui = !showGui;
		}
	}
}