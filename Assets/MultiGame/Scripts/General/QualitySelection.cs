using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class QualitySelection : MultiModule {
		
		[Tooltip("Should we save to the Player Prefs on this machine for later reference? (Works on all platforms)")]
		public bool autoSave = true;
		[Tooltip("Should we use the legacy Unity GUI? Not recommended for mobile.")]
		public bool useGUI = true;
		[Tooltip("Normalized viewport rectangle representing the portion of the screen taken up by the gui if used. Values between 0 and 1")]
		public Rect guiArea = new Rect(.8f, .3f, .19f, .2f);
		[Tooltip("Optional skin to change the look of the GUI")]
		public GUISkin guiSkin;

		public HelpInfo help = new HelpInfo("Allows the user to select the level of quality based on Unity's available levels. SetQualityLevel takes an integer");
		

		void Start () {
			if (autoSave && PlayerPrefs.HasKey("qualitySetting"))
				QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualitySetting"));
		}

		void OnGUI () {
			if (!useGUI)
				return;
			if (guiSkin != null)
				GUI.skin = guiSkin;

			GUILayout.BeginArea(new Rect(Screen.width * guiArea.x, Screen.height * guiArea.y, Screen.width * guiArea.width, Screen.height * guiArea.height), "Quality Level","box");
			string[] names = QualitySettings.names;
			GUILayout.BeginVertical();
			int i = 0;
			while (i < names.Length) {
				if (GUILayout.Button(names[i])) {
					QualitySettings.SetQualityLevel(i, true);
					if (autoSave) {
						PlayerPrefs.SetInt("qualitySetting", i);
						PlayerPrefs.Save();
					}
				}
				i++;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		public void SetQualityLevel (int _level) {
			if (_level > QualitySettings.maximumLODLevel)
				_level = QualitySettings.maximumLODLevel;
			QualitySettings.SetQualityLevel(_level);
			if (autoSave) {
				PlayerPrefs.SetInt("qualitySetting", _level);
				PlayerPrefs.Save();
			}
		}

		public void ToggleMenu () {
			useGUI = !useGUI;
		}

		public void OpenMenu () {
			useGUI = true;
		}

		public void CloseMenu () {
			useGUI = false;
		}

	}
}