using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEditorInternal;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class MGEditor : EditorWindow {
		#if UNITY_EDITOR

		protected Transform sceneTransform;
		public GameObject target;
//		private GameObject template;



		/// <summary>
		/// Adds a child object cleanly and returns it, with undo registry.
		/// </summary>
		/// <returns>The direct child.</returns>
		/// <param name="_target">_target.</param>
		public static GameObject AddDirectChild (GameObject _target) {
			GameObject _child = new GameObject("New MultiGame Object");
			Undo.RegisterCreatedObjectUndo(_child,"Create Object");
			_child.transform.SetParent(_target.transform);
			_child.transform.localPosition = Vector3.zero;
			_child.transform.localRotation = Quaternion.identity;
			_child.transform.localScale = Vector3.one;
			return _child;
		}

		/// <summary>
		/// Creates a button with an icon and optional caption
		/// </summary>
		/// <returns><c>true</c>, if button was MGed, <c>false</c> otherwise.</returns>
		/// <param name="_icon">Icon for the button.</param>
		/// <param name="_caption">Optional caption.</param>
		public static bool MGButton (Texture2D _icon, string _caption) {
			if (EditorApplication.isPlaying || _icon == null)
				return false;

			bool _ret = false;
			_ret = GUILayout.Button(_icon, GUILayout.Width(_icon.width), GUILayout.Height (_icon.height));
			if (!string.IsNullOrEmpty( _caption))
				GUILayout.Label(_caption);
			GUILayout.Space(8f);
			return _ret;
		}

		/// <summary>
		/// Makes a small button 16 pixels in size with an icon.
		/// </summary>
		/// <returns><c>true</c>, if clicked, <c>false</c> otherwise.</returns>
		/// <param name="_icon">Icon for the button.</param>
		public static bool MGPip (Texture2D _icon) {
			bool _ret = false;
			
			_ret = GUILayout.Button(_icon, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(16));

			return _ret;
		}

		/// <summary>
		/// Make a button icon with fixed layout. Does not draw the button borders.
		/// </summary>
		/// <returns><c>true</c>, when clicked, <c>false</c> otherwise.</returns>
		/// <param name="_icon">Icon.</param>
		/// <param name="_position">Position in fixed pixel coordinates.</param>
		public static bool MGPip (Texture2D _icon, Rect _position) {
			bool _ret = false;

			_ret = GUI.Button(_position,_icon, GUIStyle.none);

			return _ret;
		}

		/// <summary>
		/// Instantiates a linked prefab instead of an unlinked clone but works in the same way as Instantiate().
		/// </summary>
		/// <param name="_pFab">The prefab to be instantiated.</param>
		/// <param name="_position">Position where we will instantiate.</param>
		/// <param name="_rotation">Rotation this will be instantiated with.</param>
		public static GameObject InstantiateLinkedPrefab (GameObject _pFab, Vector3 _position, Quaternion _rotation) {
			GameObject _obj = PrefabUtility.InstantiatePrefab(_pFab) as GameObject;
			_obj.transform.position = _position;
			_obj.transform.rotation = _rotation;
			return _obj;
		}

		/// <summary>
		/// Creates an asset and returns it's path
		/// </summary>
		/// <returns>The asset path.</returns>
		/// <param name="_asset">The new asset we are creating.</param>
		/// <param name="_type">The System.Type of the asset.</param>
		public string SmartCreateAsset (UnityEngine.Object _asset, string _fileExtension) {
			if(!AssetDatabase.IsValidFolder("Assets/Generated"))
				AssetDatabase.CreateFolder("Assets","Generated");
			AssetDatabase.CreateAsset(_asset, "Assets/Generated/" + target.name + _fileExtension);
			return AssetDatabase.GetAssetPath(_asset);
		}

		protected void ResolveOrCreateTarget () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				Debug.LogWarning("Scene view inactive");
				return;
			}
			if (Selection.activeGameObject == null) {
//				if (template != null) {
//					target = Instantiate<GameObject>(template);
//					Undo.RegisterCreatedObjectUndo(target,"Create From Template");
//					string[] parts = target.name.Split('(');
//					target.name = parts[0];
//					target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
//					target.transform.rotation = Quaternion.identity;
//					Selection.activeGameObject = target;
//				} else { //No template found, create something from nothing!
					target = new GameObject("New MultiGame Object");
					Undo.RegisterCreatedObjectUndo(target,"Create New Object");
					target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
					target.transform.rotation = Quaternion.identity;
					Selection.activeGameObject = target;
//				}
			} else { //Something is selected, use that instead of creating something
				target = Selection.activeGameObject;
			}
		}

		protected void SmartRenameTarget (string _newName) {
			if (target.name == "New MultiGame Object")
				target.name = _newName;
		}

		protected void RenameTarget(string _newName) {
			target.name = _newName;
		}
		#endif
	}
}