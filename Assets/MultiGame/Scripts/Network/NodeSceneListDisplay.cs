using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class NodeSceneListDisplay : MonoBehaviour {

		public Rect guiArea = new Rect(.01f,.01f,.98f,.98f);
		public bool showGui = false;
		public string mapListUrl = "";
		public string dataUrl = "";
		public string uploadUrl = "";
		public string sceneObjectTag = "Buildable";

		public SceneObjectListSerializer serializer;

		[System.NonSerialized]
		private bool readyToDisplay = false;
		private Vector2 scrollArea = Vector2.zero;
		private string newName = "";

		[System.Serializable]
		public class NodeMapColl {
			public string[] maps;
		}

		private NodeMapColl nodeMaps;

		void Awake () {
			if (serializer == null)
				serializer = FindObjectOfType<SceneObjectListSerializer>();
		}

		void OnGUI () {
			if (!showGui)
				return;
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Refresh"))
				RefreshNodeMaps();

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("New file:");
			newName = GUILayout.TextField(newName);
			if (GUILayout.Button("Save")) {
				serializer.SetUniqueSceneIdentifier(newName);
				serializer.PopulateByTag(sceneObjectTag);
				serializer.SaveToUrl(uploadUrl);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(16f);

			if (readyToDisplay) {
				scrollArea = GUILayout.BeginScrollView(scrollArea, "box");
				for (int i = 0; i < nodeMaps.maps.Length; i++) {
					if (GUILayout.Button(nodeMaps.maps[i])) {
						serializer.ClearObjectsByTag(sceneObjectTag);
						serializer.SetUniqueSceneIdentifier(nodeMaps.maps[i]);
						serializer.LoadFromUrl(dataUrl);
					}
				}
				GUILayout.EndScrollView();
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Close"))
				DisableSceneListGui();

			GUILayout.EndArea();
		}

		public void RefreshNodeMaps () {
//			Debug.Log(JsonUtility.);
			StartCoroutine(GetNodeMaps());
		}

		public IEnumerator GetNodeMaps () {
			using(WWW www = new WWW(mapListUrl)) {
				yield return www;
				if (!string.IsNullOrEmpty( www.error)) {
					Debug.LogError(www.error);
				} else {
					nodeMaps = JsonUtility.FromJson<NodeMapColl>(www.text);
					Debug.Log (nodeMaps.ToString());
					Debug.Log(www.text);

					if (nodeMaps != null)
						Debug.Log( nodeMaps.maps.Length.ToString());
					
					readyToDisplay = true;
				}
			}
		}

		public void ToggleSceneListGUI () {
			showGui = !showGui;
		}

		public void EnableSceneListGui () {
			showGui = true;
		}

		public void DisableSceneListGui () {
			showGui = false;
		}
	}
}