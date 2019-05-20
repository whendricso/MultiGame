using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MultiGame;

namespace MultiGame {

	public class MaterialCreator : EditorWindow {
#if UNITY_EDITOR

		private Texture2D paintIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Paintbrush.png", typeof (Texture2D)) as Texture2D;


#endif
	}
}