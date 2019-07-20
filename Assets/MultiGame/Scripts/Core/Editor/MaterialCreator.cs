using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	public class MaterialCreator : EditorWindow {
#if UNITY_EDITOR
		private static Texture2D paintIcon;
#endif

		public Material mat;
		public string matName;
		public bool initialized = false;
		public Color color = Color.white;
		//public Texture2D albedo;
		//public Texture2D normal;
		//public Texture2D height;
		//public Texture2D occlusion;

		int[] sizes = new int[4] { 512,1024,2048,4096};

		[MenuItem("MultiGame/Material Creator")]
		static void ShowWindow() {
			EditorWindow creator = EditorWindow.GetWindow(typeof(MaterialCreator),true);
			creator.minSize = new Vector2(320, 320);
			paintIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Paintbrush.png", typeof(Texture2D)) as Texture2D;
		}
		void OnGUI() {
			matName = EditorGUILayout.TextField("Material Name",matName);
			if (string.IsNullOrEmpty(matName))
				matName = "Material" + Random.Range(0, 10000);
			EditorGUILayout.Space();
			//EditorGUILayout.tool
			if (!initialized) {
				if (GUILayout.Button("New Material")) {
					mat = AcquireMaterial();
					initialized = true;
				}
				return;
			}



		}

		Material AcquireMaterial() {
			Material _ret = null;

			if (mat != null)
				_ret = mat;
			else {
				if (string.IsNullOrEmpty(matName))
					matName = "Material" + Random.Range(0, 10000);
				mat = new Material(Shader.Find("Standard"));
				mat.name = matName;
				if (!Directory.Exists(Application.dataPath + "/Generated/TexMat/"))
					Directory.CreateDirectory(Application.dataPath + "/Generated/TexMat/");

				

				AssetDatabase.CreateAsset(mat, "Assets/Generated/" + matName + Mathf.RoundToInt(Random.Range(0, 100000)) + ".mesh");
				AssetDatabase.SaveAssets();
			}
			return _ret;
		}
	}
}