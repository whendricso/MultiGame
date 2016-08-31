using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class MGEditor : EditorWindow {

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
	}


}