using UnityEngine;
using UnityEditor;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class AIPhysWindow : MGEditor {

		private bool allExist = false;
		private bool showHelp = false;

		void OnGUI () {
			allExist = CheckAllExist();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("All of these must be added to the game's physics 'Layers' before MultiGame can set up the Layers automatically:");
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Player " + (LayerMask.NameToLayer("Player") != -1 ? "Exists" : "Does Not Exist") + " " + LayerMask.NameToLayer("Player") );
			EditorGUILayout.LabelField("Ally " + (LayerMask.NameToLayer("Ally") != -1 ? "Exists" : "Does Not Exist") + " " + LayerMask.NameToLayer("Ally") );
			EditorGUILayout.LabelField("Enemy " + (LayerMask.NameToLayer("Enemy") != -1 ? "Exists" : "Does Not Exist") + " " + LayerMask.NameToLayer("Enemy") );
			EditorGUILayout.LabelField("AlliedSensor " + (LayerMask.NameToLayer("AlliedSensor") != -1 ? "Exists" : "Does Not Exist") + " " + LayerMask.NameToLayer("AlliedSensor") );
			EditorGUILayout.LabelField("EnemySensor " + (LayerMask.NameToLayer("EnemySensor") != -1 ? "Exists" : "Does Not Exist") + " " + LayerMask.NameToLayer("EnemySensor") );
			EditorGUILayout.LabelField("Build " + (LayerMask.NameToLayer("Build") != -1 ? "Exists" : "Does Not Exist") + " " + LayerMask.NameToLayer("Build") );
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Click this window to refresh. The button turns green when ready.");



			GUI.backgroundColor = new Color(.2f,.2f,.91f);
			if (GUILayout.Button("Help")) {
				showHelp = !showHelp;
			}
			GUI.backgroundColor = Color.white;

			if (showHelp) {
				GUILayout.Label("This window allows you to set up Physics layers, to get the most out of Unity and MultiGame. \n" +
					"Simply click Edit -> Project Settings -> Tags and Layers \n" +
					"to add the Layers shown in this window. Unity won't refresh the window until you interact with it, \n" +
					"so click anywhere in the window, the button turns green when it's ready!"
					,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
			}

			GUI.backgroundColor = Color.white;

			GUILayout.FlexibleSpace();
			if (allExist) {
				GUI.backgroundColor = new Color(.12f, .93f,.12f);
			} else {
				GUI.backgroundColor = Color.red;
			}
			if (GUILayout.Button("Check & Initialize", GUILayout.Height(64f))) {
				allExist = CheckAllExist();
				if (allExist) {
					SetupLayerCollisions();
					Debug.Log("MultiGame set up physics layers correctly.");
				} else {
					Debug.LogError("All of the required physics layers for sensors & serialization don't exist yet, create them by clicking 'Layer' on any GameObject inspector, and click 'Add Layer' " +
						" then add each layer so that it's listed as 'Exists' click the window to refresh it. The button turns green if you click the window when all are set up correctly.");
				}
			}

			GUI.backgroundColor = Color.white;
		}

		void SetupLayerCollisions () {
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("AlliedSensor"));
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"),LayerMask.NameToLayer("EnemySensor"));
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("AlliedSensor"),LayerMask.NameToLayer("EnemySensor"));
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Build"),LayerMask.NameToLayer("EnemySensor"));
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("AlliedSensor"),LayerMask.NameToLayer("Build"));

			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("AlliedSensor"),LayerMask.NameToLayer("TransparentFX"));
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("AlliedSensor"),LayerMask.NameToLayer("Water"));

			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemySensor"),LayerMask.NameToLayer("TransparentFX"));
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemySensor"),LayerMask.NameToLayer("Water"));

		}

		bool CheckAllExist () {
			bool _ret = true;

			if (LayerMask.NameToLayer("Player") == -1)
				_ret = false;
			if (LayerMask.NameToLayer("Ally") == -1)
				_ret = false;
			if (LayerMask.NameToLayer("Enemy") == -1)
				_ret = false;
			if (LayerMask.NameToLayer("AlliedSensor") == -1)
				_ret = false;
			if (LayerMask.NameToLayer("EnemySensor") == -1)
				_ret = false;
			if (LayerMask.NameToLayer("Build") == -1)
				_ret = false;

			return _ret;
		}
	}
}