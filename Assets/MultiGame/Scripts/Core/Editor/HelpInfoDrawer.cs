using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(MultiModule.HelpInfo))]
public class HelpInfoDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		property.FindPropertyRelative("showInfo").boolValue = EditorGUI.Foldout(position, property.FindPropertyRelative("showInfo").boolValue, GUIContent.none);
		if (property.FindPropertyRelative("showInfo").boolValue)
			EditorGUI.HelpBox(position, property.FindPropertyRelative("helpText").stringValue, MessageType.Info);
		else
			EditorGUI.LabelField(position, new GUIContent("Help"));
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		if (property.FindPropertyRelative("showInfo").boolValue == true)
			return 192f;
		else
			return 16f;
	}

}
