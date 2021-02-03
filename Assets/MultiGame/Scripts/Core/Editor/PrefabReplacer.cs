using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	public class PrefabReplacer : MGEditor {

		public static GameObject original;
		public static GameObject replacement;

		//[MenuItem("MultiGame/Prefab Replacer")]
		static void ShowWindow() {
			EditorWindow replacer = EditorWindow.GetWindow(typeof(PrefabReplacer), true);
		}

		private void OnGUI() {
			GUILayout.Label("Prefab Replacer");
			GUILayout.Space(16);
			EditorGUILayout.BeginHorizontal();
			original = EditorGUILayout.ObjectField("Original",original, typeof(GameObject),false) as GameObject;
			if (GUILayout.Button("Select"))
				replacement = AcquirePrefabReplacement();
			EditorGUILayout.EndHorizontal();
			replacement = EditorGUILayout.ObjectField("Replacement", replacement, typeof(GameObject), false) as GameObject;
			if (GUILayout.Button("Replace"))
				Replace();
		}

		private GameObject AcquirePrefabReplacement() {
			return (PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.activeGameObject));//prefabutility instancehandle ?
		}

		private void Replace() {
			List<GameObject> _children = new List<GameObject>();
			GameObject _target;

			Debug.Log("Original: " + PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(original).name + " Replacement: " + PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(replacement).name);

			if (original == null || replacement == null) {
				Debug.LogError("MultiGame Prefab Replacer requires an original prefab and a replacement prefab to be assigned.");
				return;
			}

			object _targetInstanceHandle;
			object _replacementInstanceHandle = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(replacement);

			GameObject _rep;

			
			if (_replacementInstanceHandle == null) {
				Debug.LogError("MultiGame Prefab Replacer requires a prefab replacement to be selected (not an instance - please drag & drop the replacement from the Project View.");
				return;
			}

			int _childCount;
			foreach (GameObject _obj in Selection.gameObjects) {
				_childCount = _obj.transform.childCount;
				for (int i = 0; i < _obj.transform.childCount; i++) {
					_target = _obj.transform.GetChild(i).gameObject;
					_targetInstanceHandle = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(_target);
					if (_targetInstanceHandle == _replacementInstanceHandle) {
						_rep = PrefabUtility.InstantiatePrefab(replacement) as GameObject;
						Undo.RegisterCreatedObjectUndo(replacement,"Replace");
						_rep.transform.parent = _target.transform.parent;
						_rep.transform.localPosition = _target.transform.localPosition;
						_rep.transform.localRotation = _target.transform.localRotation;
						_rep.transform.localScale = _target.transform.localScale;
						Undo.DestroyObjectImmediate(_target);
					}
				}
			}
		}
	}
}