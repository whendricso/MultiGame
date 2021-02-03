using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	public class MaterialPropagator : MGEditor {

		public static Material mat = null;
		public static bool propagateChildren = true;

		[MenuItem("MultiGame/Material Propagator")]
		static void ShowWindow() {
			EditorWindow propagator = EditorWindow.GetWindow(typeof(MaterialPropagator), true);

		}

		private void OnGUI() {
			GUILayout.Label("Material");
			mat = EditorGUILayout.ObjectField(mat, typeof(Material), false) as Material;
			propagateChildren = EditorGUILayout.Toggle("Propagate Children",propagateChildren);
			if (GUILayout.Button("Propagate material to " + Selection.gameObjects.Length + " objects.")) {
				Propagate();
			}
		}

		private void Propagate() {
			List<MeshRenderer> _rends = new List<MeshRenderer>();
			foreach (GameObject _gobj in Selection.gameObjects) {
				_rends.AddRange(_gobj.GetComponentsInChildren<MeshRenderer>());
			}

			int _matCount = 0;
			Material[] _newMaterials;

			foreach (MeshRenderer _renderer in _rends) {
				_matCount = _renderer.sharedMaterials.Length;
				_newMaterials = new Material[_matCount];
				for (int i = 0; i < _matCount; i++) {
					_newMaterials[i] = mat;
				}
				_renderer.sharedMaterials = _newMaterials;
			}
		}
	}
}