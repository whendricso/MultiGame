using UnityEngine;
using System.Collections;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	[CustomPropertyDrawer(typeof(MultiModule.HelpInfo))]
	public class HelpInfoDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (property.FindPropertyRelative("showInfo").boolValue) {
				property.FindPropertyRelative("showInfo").boolValue = EditorGUI.Foldout(position, property.FindPropertyRelative("showInfo").boolValue, GUIContent.none);
				EditorGUI.HelpBox(position, property.FindPropertyRelative("helpText").stringValue, MessageType.Info);
			}
			else {
				Color _col = GUI.contentColor;
				GUI.contentColor = Color.white;
				GUI.backgroundColor = new Color(.34f,.31f,.88f,.6f);
				if (GUI.Button(position, "Help"))
					property.FindPropertyRelative("showInfo").boolValue = true;
//				EditorGUI.LabelField(position, new GUIContent("Help"));
				GUI.backgroundColor = Color.white;
				GUI.contentColor = _col;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			if (property.FindPropertyRelative("showInfo").boolValue == true)
				return 192f;
			else
				return 16f;
		}

	}
}