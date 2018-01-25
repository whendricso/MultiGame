using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	public class SnapWindow : MGEditor {

		public Vector3 gridSetting = Vector3.one;
		public bool autoSnap = false;
//		public enum SnapSpaces { Global, Local };
//		public SnapSpaces snapSpace = SnapSpaces.Global;
		private Texture2D snapIcon;



		[MenuItem ("MultiGame/Snap Window")]
		public static void ShowWindow () {
			EditorWindow.GetWindow(typeof(SnapWindow));
		}

		void OnGUI () {
			if (snapIcon == null)
				LoadIcons ();
			gridSetting = EditorGUILayout.Vector3Field ("Grid Size", gridSetting);
//			snapSpace = (SnapSpaces)EditorGUILayout.EnumPopup ("Snap Space",snapSpace);
			autoSnap = EditorGUILayout.Toggle ("Snap Automatically",autoSnap);
			if (MGButton (snapIcon, "Snap\nSelected")) {
				SnapSelection ();
			}
		}

		void Update () {
			if (autoSnap)
				SnapSelection ();
		}

		void LoadIcons () {
			snapIcon = Resources.Load<Texture2D> ("Snap");
		}

		void SnapSelection() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects (Selection.gameObjects, "Snap");
				foreach (GameObject gobj in Selection.gameObjects) {
					SnapTargetToGrid (gobj, gridSetting);
				}
			}
		}

		public void SnapTargetToGrid (GameObject _target, Vector3 _gridSpace) {
			if (float.IsNaN (_gridSpace.x) || float.IsNaN (_gridSpace.y) || float.IsNaN (_gridSpace.z))
				return;
			if (_gridSpace.x == 0 || _gridSpace.y == 0 || _gridSpace.z == 0)
				return;
			float newX;
			float newY;
			float newZ;

//			if (snapSpace == SnapSpaces.Local) {
//				newX = Mathf.Round (_target.transform.localPosition.x / _gridSpace.x) * _gridSpace.x;
//				newY = Mathf.Round (_target.transform.localPosition.y / _gridSpace.y) * _gridSpace.y;
//				newZ = Mathf.Round (_target.transform.localPosition.z / _gridSpace.z) * _gridSpace.z;
//			} else {
				newX = Mathf.Round (_target.transform.position.x / _gridSpace.x) * _gridSpace.x;
				newY = Mathf.Round (_target.transform.position.y / _gridSpace.y) * _gridSpace.y;
				newZ = Mathf.Round (_target.transform.position.z / _gridSpace.z) * _gridSpace.z;
//			}
			_target.transform.position = new Vector3(newX, newY, newZ);
		}

	}
}