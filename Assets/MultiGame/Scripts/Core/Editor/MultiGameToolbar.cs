using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

//context-sensitive toolbar
//Creation Workflow:
//1. Create object by type
//2. Add common components
//3. Create variations

namespace MultiGame {

	public class MultiGameToolbar : EditorWindow {

		public enum Modes {Create, SceneObject};
		public Modes mode = Modes.Create;

		public GameObject template;

		private GameObject target;


		[MenuItem ("Window/MultiGame/Rapid Dev Tool")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(MultiGameToolbar));
		}

		void OnGUI () {

			switch (mode) {
			case Modes.Create:
				CreateObjectGUI();
				break;
			case Modes.SceneObject:
				SceneObjectGUI();
				break;
			}

		}

		void OnInspectorUpdate () {
			if (Selection.transforms.Length > 0)
				mode = Modes.SceneObject;
			else
				mode = Modes.Create;
		}

		void CreateObjectGUI () {
			EditorGUILayout.BeginVertical("box");
			template = EditorGUILayout.ObjectField(template, typeof(GameObject), true, GUILayout.Width(64f), GUILayout.Height(16f)) as GameObject;
			EditorGUILayout.LabelField("Template");
			EditorGUILayout.EndVertical();

		}

		void SceneObjectGUI () {

		}

		public void CreateFromTemplate() {
//			target = Instantiate(
		}

		bool MGButton (Texture2D _icon, string _caption) {
			bool _ret = false;
			EditorGUILayout.BeginVertical("box");
			_ret = GUILayout.Button(_icon);
			GUILayout.Label(_caption);
			EditorGUILayout.EndVertical();
			return _ret;
		}
	}
}