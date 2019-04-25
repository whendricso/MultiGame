using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class RapidDevTool : MGEditor {
#if UNITY_EDITOR



		//[MenuItem("MultiGame/Toolbar (Legacy)")]
		public static void ShowWindow() {
			EditorWindow window = EditorWindow.GetWindow(typeof(MultiGameToolbar));
			window.minSize = new Vector2(116f, 640f);
			window.maxSize = new Vector2(116f, Mathf.Infinity);
		}


#endif
	}
}