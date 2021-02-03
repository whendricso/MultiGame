using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	[CustomPropertyDrawer(typeof(SceneVector))]
	public class SceneVectorDrawer : PropertyDrawer {

		Vector3 newTargetPosition;

		SerializedProperty prop;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			prop = property;
			/*
			
				EditorGUI.BeginChangeCheck();
				Debug.Log("bleep");
				newTargetPosition = Handles.PositionHandle(prop.vector3Value, Quaternion.identity);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(prop.serializedObject.targetObject, "Moved Vector Position");
					prop.vector3Value = newTargetPosition;
				}
			}
			*/
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return 0;
		}
	}
}