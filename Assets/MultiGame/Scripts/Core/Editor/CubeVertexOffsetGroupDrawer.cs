using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomPropertyDrawer(typeof(ProcCube.VertexOffsetGroup))]
public class CubeVertexOffsetGroupDrawer : PropertyDrawer {
	private bool opened = false;
	private Rect currentLinePosition;
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		currentLinePosition = new Rect (position);
		position.height = 16;
		opened = EditorGUI.Foldout (position, opened, "Vertex Offsets");
		if (opened) {
			currentLinePosition.y += 16;
			property.FindPropertyRelative ("topFrontLeft").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Top Front Left", property.FindPropertyRelative ("topFrontLeft").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("topBackLeft").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Top Back Left", property.FindPropertyRelative ("topBackLeft").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("topFrontRight").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Top Front Right", property.FindPropertyRelative ("topFrontRight").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("topBackRight").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Top Back Right", property.FindPropertyRelative ("topBackRight").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("bottomFrontLeft").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Bottom Front Left", property.FindPropertyRelative ("bottomFrontLeft").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("bottomBackLeft").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Bottom Back Left", property.FindPropertyRelative ("bottomBackLeft").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("bottomFrontRight").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Bottom Front Right", property.FindPropertyRelative ("bottomFrontRight").vector3Value);
			currentLinePosition.y += 32;
			property.FindPropertyRelative ("bottomBackRight").vector3Value = EditorGUI.Vector3Field (currentLinePosition, "Bottom Back Right", property.FindPropertyRelative ("bottomBackRight").vector3Value);
		}
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		if (opened)
			return 272;
		else
			return 16;
	}

}
