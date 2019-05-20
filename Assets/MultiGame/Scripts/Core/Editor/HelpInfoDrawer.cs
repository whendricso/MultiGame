using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	[CustomPropertyDrawer(typeof(MultiModule.HelpInfo))]
	public class HelpInfoDrawer : PropertyDrawer {
		#if UNITY_EDITOR
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (property.FindPropertyRelative("showInfo").boolValue) {//help info is opened, show information
				property.FindPropertyRelative("showInfo").boolValue = EditorGUI.Foldout(position, property.FindPropertyRelative("showInfo").boolValue, GUIContent.none);
				EditorGUI.HelpBox(position, property.FindPropertyRelative("helpText").stringValue, MessageType.Info);
			}
			else {//help info is closed, show buttons
				Color _col = GUI.contentColor;
				GUI.contentColor = Color.white;
				GUI.backgroundColor = new Color(.34f,.31f,.88f,.6f);
				if (string.IsNullOrEmpty(property.FindPropertyRelative("videoLink").stringValue)) {//no video link, hide video button
					if (GUI.Button(position, "Help"))
						property.FindPropertyRelative("showInfo").boolValue = true;
				} else {//video link found, show video button
					if (GUI.Button(new Rect( position.x, position.y, position.width - 64, position.height ), "Help"))
						property.FindPropertyRelative("showInfo").boolValue = true;
					if (GUI.Button(new Rect(position.x + (position.width - 62), position.y, 62, position.height) , WebHelpLoader.videoIcon) )
						Application.OpenURL(property.FindPropertyRelative("videoLink").stringValue);
				}
//				EditorGUI.LabelField(position, new GUIContent("Help"));
				GUI.backgroundColor = Color.white;
				GUI.contentColor = _col;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			if (property.FindPropertyRelative("showInfo").boolValue == true)
				return 192f;
			else
				return 32f;
		}
		#endif
	}
}