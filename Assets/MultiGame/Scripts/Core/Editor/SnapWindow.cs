using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	public class SnapWindow : MGEditor {

		public Vector3 gridSetting = Vector3.one;
		public Vector3 rotationSetting = new Vector3(15,15,15);
		public bool autoSnap = false;
		public bool autoSnapRotation = false;
		[System.NonSerialized]
		private Texture2D snappingIcon;

		[MenuItem ("MultiGame/Snap Window")]
		public static void ShowWindow () {
			EditorWindow.GetWindow(typeof(SnapWindow));
		}

		void OnGUI () {
			if (snappingIcon == null)
				LoadIcons();
			gridSetting = EditorGUILayout.Vector3Field ("Grid Size", gridSetting);
			rotationSetting = EditorGUILayout.Vector3Field ("Rotation Snap", rotationSetting);
			EditorGUILayout.BeginHorizontal();
			autoSnap = EditorGUILayout.Toggle ("Snap To Grid",autoSnap);
			autoSnapRotation = EditorGUILayout.Toggle ("Snap Rotation",autoSnapRotation);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (MGButton (snappingIcon, "Snap\nPosition")) {
				SnapSelection ();
			}
			if (MGButton(snappingIcon, "Snap\nRotation")) {
				SnapRotation();
			}
			EditorGUILayout.EndHorizontal();
		}

		void Update () {
			if (autoSnap)
				SnapSelection ();
			if (autoSnapRotation)
				SnapRotation();
		}

		void LoadIcons () {
			Debug.Log("Loading icon");
			snappingIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Snap.png", typeof(Texture2D)) as Texture2D;
		}

		void SnapSelection() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects (Selection.gameObjects, "Snap");
				foreach (GameObject gobj in Selection.gameObjects) {
					SnapTargetToGrid (gobj, gridSetting);
				}
			}
		}

		void SnapRotation() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Snap");
				foreach (GameObject gobj in Selection.gameObjects) {
					SnapTargetToRotation(gobj, rotationSetting);
				}
			}
		}

		public void SnapTargetToGrid (GameObject _target, Vector3 _gridSpace) {
			if (float.IsNaN (_gridSpace.x) || float.IsNaN (_gridSpace.y) || float.IsNaN (_gridSpace.z))
				return;
			float newX;
			float newY;
			float newZ;

			newX = Mathf.Round (_target.transform.position.x / _gridSpace.x) * _gridSpace.x;
			newY = Mathf.Round (_target.transform.position.y / _gridSpace.y) * _gridSpace.y;
			newZ = Mathf.Round (_target.transform.position.z / _gridSpace.z) * _gridSpace.z;
			_target.transform.position = new Vector3(newX, newY, newZ);
		}

		public void SnapTargetToRotation(GameObject _target, Vector3 _rotationSetting) {
			if (float.IsNaN(_rotationSetting.x) || float.IsNaN(_rotationSetting.y) || float.IsNaN(_rotationSetting.z))
				return;
			float newX;
			float newY;
			float newZ;

			newX = Mathf.Round(_target.transform.eulerAngles.x / _rotationSetting.x) * _rotationSetting.x;
			newY = Mathf.Round(_target.transform.eulerAngles.y / _rotationSetting.y) * _rotationSetting.y;
			newZ = Mathf.Round(_target.transform.eulerAngles.z / _rotationSetting.z) * _rotationSetting.z;
			_target.transform.eulerAngles = new Vector3(newX, newY, newZ);
		}

	}
}