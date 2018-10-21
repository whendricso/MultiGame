using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MultiGame;
using System.Collections;
using System.Collections.Generic;

namespace MultiGame {
	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(RequiredFieldAttribute))]
	#endif
	public class RequiredPropertyDrawer : PropertyDrawer {
		#if UNITY_EDITOR
		bool showHelp;
		Rect fieldPosition;
		Rect helpButtonPosition;
		Rect helpAreaPosition;
		Rect levelLabelPosition;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {

			levelLabelPosition = new Rect(position.x, position.y - 18f, 96f, position.height);
			fieldPosition = new Rect( position.x/* + 98f*/, position.y, position.width - 16f, 16f);
			helpButtonPosition = new Rect(Mathf.Abs(2f + position.width), position.y, 16f, 16f);
			helpAreaPosition = new Rect(position.x + 98f, position.y + 18f, position.width - 116f, position.height - 18f);

			RequiredFieldAttribute required = attribute as RequiredFieldAttribute;

			if (property.propertyType == SerializedPropertyType.Float) {
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Optional) {
					GUI.color = MGEditor.validColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Optional");
					if (property.floatValue == 0f) {
						GUI.color = MGEditor.validColor;
					}
					else {
						GUI.color = Color.white;
					}
				}
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Recommended) {
					GUI.color = MGEditor.warningColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Recommended");
					if (property.floatValue == 0f) {
						GUI.color = MGEditor.warningColor;
					}
					else {
						GUI.color = Color.white;
					}
				}
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Required) {
					GUI.color = MGEditor.errorColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Required");
					if (property.floatValue == 0f) {
						GUI.color = MGEditor.errorColor;
					}
					else {
						GUI.color = MGEditor.affirmationColor;
					}
				}

				property.floatValue = EditorGUI.FloatField(fieldPosition, property.displayName, property.floatValue);
			}
			if (property.propertyType == SerializedPropertyType.Integer) {
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Optional) {
					GUI.color = MGEditor.validColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Optional");
					if (property.intValue == 0) {
						GUI.color = MGEditor.validColor;
					}
					else {
						GUI.color = Color.white;
					}
				}
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Recommended) {
					GUI.color = MGEditor.warningColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Recommended");
					if (property.intValue == 0) {
						GUI.color = MGEditor.warningColor;
					}
					else {
						GUI.color = Color.white;
					}
				}if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Required) {
					GUI.color = MGEditor.errorColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Required");
					if (property.intValue == 0) {
						GUI.color = MGEditor.errorColor;
					}
					else {
						GUI.color = MGEditor.affirmationColor;
					}
				}
					
				property.intValue = EditorGUI.IntField(fieldPosition, property.displayName, property.intValue);
			}
			if (property.propertyType == SerializedPropertyType.String) {
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Optional) {
					GUI.color = MGEditor.validColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Optional");
					if (string.IsNullOrEmpty( property.stringValue)) {
						GUI.color = MGEditor.validColor;
					}
					else {
						GUI.color = Color.white;
					}
				} 
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Recommended) {
					GUI.color = MGEditor.warningColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Recommended");
					if (string.IsNullOrEmpty( property.stringValue)) {
						GUI.color = MGEditor.warningColor;
					}
					else {
						GUI.color = Color.white;
					}
				}
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Required) {
					GUI.color = MGEditor.errorColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Required");
					if (string.IsNullOrEmpty( property.stringValue)) {
						GUI.color = MGEditor.errorColor;
					}
					else {
						GUI.color = MGEditor.affirmationColor;
					}
				}
				property.stringValue = EditorGUI.TextField(fieldPosition, property.displayName, property.stringValue);
			}
			if (property.propertyType == SerializedPropertyType.ObjectReference) {
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Optional) {
					GUI.color = MGEditor.validColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Optional");
					if (property.objectReferenceValue == null) {
						GUI.color = MGEditor.validColor;
					}
					else {
						GUI.color = Color.white;
					}
				}
				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Recommended) {
					GUI.color = MGEditor.warningColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Recommended");
					if (property.objectReferenceValue == null) {
						GUI.color = MGEditor.warningColor;
					}
					else {
						GUI.color = Color.white;
					}
				}if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Required) {
					GUI.color = MGEditor.errorColor;
					if (showHelp)
						EditorGUI.DropShadowLabel(levelLabelPosition, "Required");
					if (property.objectReferenceValue == null) {
						GUI.color = MGEditor.errorColor;
					}
					else {
						GUI.color = MGEditor.affirmationColor;
					}
				}

				EditorGUI.ObjectField(fieldPosition, property,new GUIContent( property.displayName));
			}
//			if (property.isArray) {//TODO: Does not work!
//				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Optional) {
//					GUI.color = MGEditor.validColor;
//					if (showHelp)
//						EditorGUI.DropShadowLabel(levelLabelPosition, "Optional");
//					if (property.objectReferenceValue == null) {
//						GUI.color = MGEditor.validColor;
//					}
//					else {
//						GUI.color = MGEditor.affirmationColor;
//					}
//				}
//				if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Recommended) {
//					GUI.color = MGEditor.warningColor;
//					if (showHelp)
//						EditorGUI.DropShadowLabel(levelLabelPosition, "Recommended");
//					if (property.objectReferenceValue == null) {
//						GUI.color = MGEditor.warningColor;
//					}
//					else {
//						GUI.color = MGEditor.affirmationColor;
//					}
//				}if (required.requirementLevel == RequiredFieldAttribute.RequirementLevels.Required) {
//					GUI.color = MGEditor.errorColor;
//					if (showHelp)
//						EditorGUI.DropShadowLabel(levelLabelPosition, "Required");
//					if (property.objectReferenceValue == null) {
//						GUI.color = MGEditor.errorColor;
//					}
//					else {
//						GUI.color = MGEditor.affirmationColor;
//					}
//				}
//
//				//(fieldPosition, property, new GUIContent( property.displayName, property.tooltip));
//			}

			GUI.color = Color.white;


			if (GUI.Button(helpButtonPosition,"?"))
				showHelp = !showHelp;
			if (showHelp)
				EditorGUI.HelpBox(helpAreaPosition, this.attribute.GetType().GetField("helpText").GetValue(attribute).ToString(), MessageType.Info);

		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			if (!showHelp)
				return 16;
			else
				return 96;
		}
		#endif
	}
}