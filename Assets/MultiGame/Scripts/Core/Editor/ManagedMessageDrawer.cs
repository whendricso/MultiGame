using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame
{

	[CustomPropertyDrawer (typeof(MessageManager.ManagedMessage))]
	public class ManagedMessageDrawer : PropertyDrawer
	{
		
		string[] possibleMessages;
		List<string> _msgStrings;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.color = Color.white;

			EditorGUI.BeginProperty (position, label, property);
			position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);//Really?
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;


			Rect targetRect = new Rect (position.x-32, position.y, position.width+32, 16f);
			Rect messageRect = new Rect (position.x-64, position.y + 16f, position.width+64, 16f);
			Rect sendTypeRect = new Rect (position.x-128, position.y + 80f, position.width+128, 16f);
			Rect parameterRect = new Rect (position.x-128, position.y + 48f, position.width+128, 16f);
			Rect parameterTypeRect = new Rect (position.x-128, position.y + 64f, position.width+128, 16f);
			Rect rescanButtonRect = new Rect (position.x-128, position.y + 32f, position.width+128, 16f);//Wow it actually works xD


			_msgStrings = new List<string> ();
			_msgStrings.Add ("--none--");
			for (int i = 0; i < property.FindPropertyRelative ("possibleMessages").arraySize; i++) {
				string val = property.FindPropertyRelative ("possibleMessages").GetArrayElementAtIndex (i).stringValue;
				if (!val.Contains("_"))
					_msgStrings.Add (val);
			}
			possibleMessages = _msgStrings.ToArray ();

			if (property.FindPropertyRelative("target").objectReferenceValue == null)
				GUI.color = MGEditor.validColor;
			else
				GUI.color = Color.white;
			EditorGUI.PropertyField (targetRect, property.FindPropertyRelative ("target"),GUIContent.none/*, new GUIContent ("Target")*/);
				
			//display as a string if the override is enabled, this also locks the message in so it won't change if the list is rebuilt
			if (property.FindPropertyRelative ("msgOverride").boolValue) {
				if (_msgStrings.Contains (property.FindPropertyRelative ("message").stringValue)) {
					if (string.Equals(property.FindPropertyRelative("message").stringValue, "--none--"))
						GUI.color = Color.white;
					else
						GUI.color = MGEditor.affirmationColor;
				}
				else
					GUI.color = MGEditor.validColor;
				EditorGUI.PropertyField (messageRect, property.FindPropertyRelative ("message"), GUIContent.none);
				GUI.color = Color.white;
			} else {
				if (possibleMessages.Length > 0) {
					if (property.FindPropertyRelative ("possibleMessages").arraySize < property.FindPropertyRelative ("messageIndex").intValue)
						property.FindPropertyRelative ("messageIndex").intValue = 0;
					if (string.Equals( property.FindPropertyRelative("message").stringValue, "--none--"))
						GUI.color = Color.white;
					else
						GUI.color = MGEditor.warningColor;
					property.FindPropertyRelative ("messageIndex").intValue = EditorGUI.Popup (messageRect, property.FindPropertyRelative ("messageIndex").intValue, possibleMessages);
					GUI.color = Color.white;
					property.FindPropertyRelative ("message").stringValue = possibleMessages [property.FindPropertyRelative ("messageIndex").intValue];
					if (GUI.Button (rescanButtonRect, "Refresh Messages"))
						property.FindPropertyRelative ("isDirty").boolValue = true;
						
				} else {
					property.FindPropertyRelative ("isDirty").boolValue = EditorGUI.Toggle (messageRect, new GUIContent ("Rescan For Messages"), property.FindPropertyRelative ("isDirty").boolValue);
				}
			}
			if (property.FindPropertyRelative ("msgOverride").boolValue)
				GUI.color = MGEditor.validColor;
			else {
				if (string.Equals( property.FindPropertyRelative("message").stringValue, "--none--"))
					GUI.color = Color.white;
				else
					GUI.color = MGEditor.warningColor;
			}
			if (!property.FindPropertyRelative("msgOverride").boolValue)
				EditorGUI.LabelField (new Rect (/*messageRect.width - messageRect.x * */30, messageRect.y, messageRect.width * .2f, messageRect.height),"Lck");
			EditorGUI.PropertyField (new Rect (/*messageRect.width - messageRect.x * */54, messageRect.y, messageRect.width * .2f, messageRect.height), property.FindPropertyRelative ("msgOverride"), GUIContent.none);
			GUI.color = Color.white;
			if ((property.FindPropertyRelative ("message").stringValue != "--none--" && !string.IsNullOrEmpty (property.FindPropertyRelative ("message").stringValue)) && property.FindPropertyRelative ("msgOverride").boolValue != false) {
				EditorGUI.PropertyField (sendTypeRect, property.FindPropertyRelative ("sendMessageType"), new GUIContent ("Send Mode"));
				EditorGUI.PropertyField (parameterRect, property.FindPropertyRelative ("parameter"), new GUIContent ("Parameter"));
				if (!string.IsNullOrEmpty (property.FindPropertyRelative ("parameter").stringValue))
					EditorGUI.PropertyField (parameterTypeRect, property.FindPropertyRelative ("parameterMode"), new GUIContent ("Parameter Mode"));
			} else {
				if (property.FindPropertyRelative ("message").stringValue != "--none--" && !string.IsNullOrEmpty (property.FindPropertyRelative ("message").stringValue)) {
					if (property.FindPropertyRelative ("msgOverride").boolValue)
						EditorGUI.HelpBox (new Rect (rescanButtonRect.x * .2f, rescanButtonRect.y, rescanButtonRect.width / 2f, rescanButtonRect.height * 3f), "Locked. Will save & can be edited.", MessageType.Info);
					else
						EditorGUI.HelpBox (new Rect (rescanButtonRect.x, rescanButtonRect.y + 16f, rescanButtonRect.width, rescanButtonRect.height * 3f), "Lock message to save it.", MessageType.Warning);

					//if (property.FindPropertyRelative("msgOverride").boolValue) {

					//}


				} /*else {
					if (!property.FindPropertyRelative ("msgOverride").boolValue)
						EditorGUI.HelpBox (new Rect (rescanButtonRect.x * .2f, rescanButtonRect.y, rescanButtonRect.width / 2f, rescanButtonRect.height * 2f), "Select a message to get started.", MessageType.Info);
					else
						EditorGUI.HelpBox (new Rect (rescanButtonRect.x * .2f, rescanButtonRect.y, rescanButtonRect.width / 2f, rescanButtonRect.height * 2f), "Locked. No message will be sent.", MessageType.Info);
				}*/
			}

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty ();
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			if (property == null)
				return 80;
			string _val = property.FindPropertyRelative ("message").stringValue;
			if (_val != "--none--" && !string.IsNullOrEmpty (_val))
				return 112;
			else {
				if (!property.FindPropertyRelative("msgOverride").boolValue)
					return 80;
				else
					return 50;
			}
		}
	}
}
//MultiGame is Copyright William Hendrickson and Tech Drone, all rights reserved.