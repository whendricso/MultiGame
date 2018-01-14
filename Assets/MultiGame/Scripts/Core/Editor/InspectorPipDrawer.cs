using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	[CustomPropertyDrawer(typeof(InspectorPip))]
	public class InspectorPipDrawer : PropertyDrawer {

		InspectorPip pip;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {

			pip = attribute as InspectorPip;

			if (GUI.Button(new Rect(position.x, position.y, 24f, position.height), pip.icon, GUIStyle.none)) {
				property.boolValue = true;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return 24f;
		}
	}
}