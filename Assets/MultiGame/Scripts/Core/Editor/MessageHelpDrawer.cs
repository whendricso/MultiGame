using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using MultiGame;

namespace MultiGame {

	[CustomPropertyDrawer(typeof(MultiModule.MessageHelp))]
	public class MessageHelpDrawer : PropertyDrawer {
		#if UNITY_EDITOR
		int argType;//{None, Bool, Int, Float, String}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {

			if (property.FindPropertyRelative("showInfo").boolValue)
				property.FindPropertyRelative("showInfo").boolValue = EditorGUI.Foldout(position, property.FindPropertyRelative("showInfo").boolValue, GUIContent.none);
			else
				property.FindPropertyRelative("showInfo").boolValue = GUI.Button(position,"");

			if (property.FindPropertyRelative("showInfo").boolValue) {
				argType = property.FindPropertyRelative("argumentType").intValue;

				GUI.color = Color.cyan;
				EditorGUI.DropShadowLabel(new Rect(position.x, position.y, position.width, 16f), property.FindPropertyRelative("messageName").stringValue);
				GUI.color = Color.white;
				EditorGUI.HelpBox(new Rect( position.x, position.y + 18f, position.width, position.height - 100f), property.FindPropertyRelative("helpText").stringValue, MessageType.Info);

				if (argType == 0) {
					EditorGUI.DropShadowLabel(new Rect(position.x, (position.y + position.height) - 82f, position.width, 16f), "No parameter.");
				} else {
					if (argType == 1) {
						GUI.color = Color.yellow;
						EditorGUI.DropShadowLabel(new Rect(position.x, (position.y + position.height) - 82f, position.width, 16f), "Takes a 'Boolean' parameter.");
						GUI.color = Color.white;
						EditorGUI.HelpBox(new Rect(position.x, (position.y + position.height) - 64f, position.width, 64f), property.FindPropertyRelative("argumentText").stringValue, MessageType.Info);
					}
					if (argType == 2) {
						GUI.color = Color.yellow;
						EditorGUI.DropShadowLabel(new Rect(position.x, (position.y + position.height) - 82f, position.width, 16f), "Takes an 'Integer' parameter.");
						GUI.color = Color.white;
						EditorGUI.HelpBox(new Rect(position.x, (position.y + position.height) - 64f, position.width, 64f), property.FindPropertyRelative("argumentText").stringValue, MessageType.Info);
					}
					if (argType == 3) {
						GUI.color = Color.yellow;
						EditorGUI.DropShadowLabel(new Rect(position.x, (position.y + position.height) - 82f, position.width, 16f), "Takes a 'Float' parameter.");
						GUI.color = Color.white;
						EditorGUI.HelpBox(new Rect(position.x, (position.y + position.height) - 64f, position.width, 64f), property.FindPropertyRelative("argumentText").stringValue, MessageType.Info);
					}
					if (argType == 4) {
						GUI.color = Color.yellow;
						EditorGUI.DropShadowLabel(new Rect(position.x, (position.y + position.height) - 82f, position.width, 16f), "Takes a 'String' parameter.");
						GUI.color = Color.white;
						EditorGUI.HelpBox(new Rect(position.x, (position.y + position.height) - 64f, position.width, 64f), property.FindPropertyRelative("argumentText").stringValue, MessageType.Info);
					}
				}
			}
			else {
				EditorGUI.LabelField(position, new GUIContent("Msg: " + property.FindPropertyRelative("messageName").stringValue));
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			if (property.FindPropertyRelative("showInfo").boolValue == true) {
					return 224f;
			}
			else
				return 16f;
		}
		#endif
	}
}