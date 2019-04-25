using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	public class SnapWindow : MGEditor {
#if UNITY_EDITOR

		public Vector3 gridSetting = Vector3.one;
		public Vector3 rotationSetting = new Vector3(15,15,15);
		public bool autoSnap = false;
		public bool autoSnapRotation = false;
		[System.NonSerialized]
		private Texture2D snappingIcon;
		private Texture2D snapForwardIcon;
		private Texture2D snapZeroIcon;
		private Texture2D snapDownIcon;

		[MenuItem ("MultiGame/Snap Window")]
		public static void ShowWindow () {
			EditorWindow.GetWindow(typeof(SnapWindow));
		}

		void OnGUI () {
			if (EditorApplication.isPlaying)
				return;
			if (snappingIcon == null)
				LoadIcons();
			gridSetting = EditorGUILayout.Vector3Field ("Grid Size", gridSetting);
			rotationSetting = EditorGUILayout.Vector3Field ("Rotation Snap", rotationSetting);
			EditorGUILayout.BeginVertical();
			autoSnap = EditorGUILayout.Toggle ("Snap To Grid",autoSnap);
			autoSnapRotation = EditorGUILayout.Toggle ("Snap Rotation",autoSnapRotation);
			EditorGUILayout.BeginHorizontal();
			if (MGButton (snappingIcon, "Snap\nPosition")) {
				SnapSelection ();
			}
			if (MGButton(snapForwardIcon, "Snap\nRotation")) {
				SnapRotation();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (MGButton(snapDownIcon, "Snap to \nSurface"))
				SnapDown();

			EditorGUILayout.BeginVertical();
			if (GUILayout.Button("Zero Local Position"))
				ZeroLocal();
			if (GUILayout.Button("Zero Local Rotation"))
				FaceLocal();
			if (GUILayout.Button("Zero Global Position"))
				ZeroGlobal();
			if (GUILayout.Button("Zero Global Rotation"))
				FaceGlobal();
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		void Update () {
			if (EditorApplication.isPlaying)
				return;
			if (autoSnap)
				SnapSelection ();
			if (autoSnapRotation)
				SnapRotation();
		}

		void LoadIcons () {
			snappingIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Snap.png", typeof(Texture2D)) as Texture2D;
			snapDownIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SnapToSurface.png", typeof(Texture2D)) as Texture2D;
			snapForwardIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SnapRotation.png", typeof(Texture2D)) as Texture2D;
			snapZeroIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SnapToZero.png", typeof(Texture2D)) as Texture2D;
		}

		void ZeroLocal() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Snap Zero Local");
				foreach (GameObject gobj in Selection.gameObjects) {
					gobj.transform.localPosition = Vector3.zero;
				}
			}
		}

		void FaceLocal() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Face Zero Local");
				foreach (GameObject gobj in Selection.gameObjects) {
					gobj.transform.localRotation = Quaternion.identity;
				}
			}
		}

		void ZeroGlobal() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Snap Zero Global");
				foreach (GameObject gobj in Selection.gameObjects) {
					gobj.transform.position = Vector3.zero;
				}
			}
		}

		void FaceGlobal() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Face Zero Global");
				foreach (GameObject gobj in Selection.gameObjects) {
					gobj.transform.rotation = Quaternion.identity;
				}
			}
		}

		void SnapSelection() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects (Selection.gameObjects, "Snap Position");
				foreach (GameObject gobj in Selection.gameObjects) {
					SnapTargetToGrid (gobj, gridSetting);
				}
			}
		}

		void SnapRotation() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Snap Rotation");
				foreach (GameObject gobj in Selection.gameObjects) {
					SnapTargetToRotation(gobj, rotationSetting);
				}
			}
		}

		void SnapDown() {
			if (Selection.gameObjects.Length > 0) {
				Undo.RecordObjects(Selection.gameObjects, "Snap To Surface");
				foreach (GameObject gobj in Selection.gameObjects) {
					SnapTargetToSurface(gobj);
				}
			}
		}

		public void SnapTargetToSurface(GameObject _target) {
			RaycastHit _hinfo;
			bool didHit = Physics.Raycast(_target.transform.position, Vector3.down,out _hinfo);
			if (didHit)
				_target.transform.position = _hinfo.point;
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
#endif

	}
}