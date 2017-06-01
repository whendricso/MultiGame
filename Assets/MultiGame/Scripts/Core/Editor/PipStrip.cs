using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class PipStrip : MGEditor {
		#if UNITY_EDITOR
		private int mode = 0;
		private bool showHelp = false;

		[MenuItem ("Window/MultiGame/Object Creation Tool")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(PipStrip));
		}

		void OnGUI () {
			EditorGUILayout.BeginHorizontal();

			mode = GUILayout.Toolbar(mode, new Texture[] {Resources.Load<Texture2D>("NewCube"), Resources.Load<Texture2D>("NewSphere")}, GUILayout.Height(32f) );

			EditorGUILayout.EndHorizontal();
		}
		#endif
	}
}