using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System.Collections;

[CustomPropertyDrawer(typeof(ReorderableField))]
public class ReorderableFieldDrawer : PropertyDrawer {

	private ReorderableList reorderable;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		if (reorderable == null) {
			reorderable = new ReorderableList(property.objectReferenceValue as IList, property.GetType(), true, true, true, true);
		}
	}
}
