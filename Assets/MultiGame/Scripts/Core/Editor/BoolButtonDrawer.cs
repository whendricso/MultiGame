using UnityEngine;
using UnityEditor;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[CustomPropertyDrawer(typeof(BoolButton))]
	public class BoolButtonDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (GUI.Button(position, property.displayName)) {
				property.boolValue = true;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return 32f;
		}
	}
}