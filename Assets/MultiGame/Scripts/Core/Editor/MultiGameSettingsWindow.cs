using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {
	[InitializeOnLoad]
	public class MultiGameSettingsWindow : MGEditor {

		[System.NonSerialized]
		private static bool settingsLoaded = false;
		private static Color err;
		private static Color warning;
		private static Color valid;
		private static Color affirmation;
		

		[MenuItem("MultiGame/Experimental/MultiGame Settings")]
		public static void ShowWindow() {
			EditorWindow.GetWindow(typeof(MultiGameSettingsWindow));
		}

		static MultiGameSettingsWindow() {
			Debug.Log("Settings initialized");
			if (!settingsLoaded) {
				LoadSettings();
			}
		}

		private void OnGUI() {
			EditorGUILayout.LabelField("Inspector Colors");
			err = EditorGUILayout.ColorField("Error", err);
			warning = EditorGUILayout.ColorField("Warning", warning);
			valid = EditorGUILayout.ColorField("Valid", valid);
			affirmation = EditorGUILayout.ColorField("Affirmative", affirmation);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Button Sizes");
			mgButtonSize = EditorGUILayout.IntField("MG Buttons",mgButtonSize);
			mgPipSize = EditorGUILayout.IntField("MG Pips", mgPipSize);
			EditorGUILayout.HelpBox(new GUIContent("MG Buttons are the large graphical buttons with a text label, mostly found on the Rapid Dev Tool. MG Pips are the small icons without a lable or border, just an icon."));

			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Revert"))
				LoadSettings();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Save"))
			EditorGUILayout.EndHorizontal();
		}

		private static void LoadSettings() {

			if (EditorPrefs.HasKey("MGerrorColor")) {
				err = LoadColor("MGerrorColor");
				errorColor = err;
				warning = LoadColor("MGwarningColor");
				warningColor = warning;
				valid = LoadColor("MGvalidColor");
				validColor = valid;
				affirmation = LoadColor("MGaffirmationColor");
				affirmationColor = affirmation;
			}

			settingsLoaded = true;
		}

		private static void SaveSettings() {
			SaveColor(err, "MGerrorColor");
			SaveColor(warning, "MGwarningColor");
			SaveColor(valid, "MGvalidColor");
			SaveColor(affirmation, "MGaffirmationColor");
		}
		
	}
}