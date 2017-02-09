using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Transform Window")]
	public class TransformWindow : MultiModule {

		[Tooltip("Unique identifier for the legacy Unity GUI window")]
		public int windowID = 1234;
		[Tooltip("Normalized viewport rectangle indicating the area for the legacy Unity GUI window")]
		public Rect guiArea = new Rect(0.8f, .4f, .19f, .4f);
		public GUISkin guiSkin;
		[Tooltip("Collision layers of objects that we can transform")]
		public LayerMask rayMask;
		[Tooltip("Mouse button for activating transform on an object")]
		public int mouseButton = 2;

		[HideInInspector]
		public Transformer transformer;
		[HideInInspector]
		public GameObject target;
		[HideInInspector]
		public float gridSize = 1.0f;
		private string[] mode = new string[3];
		private string[] axis = new string[4];
		private string[] space = new string[2];
		private int currentMode = 0;
		private int currentAxis = 0;
		private int currentSpace = 1;

		public HelpInfo help = new HelpInfo("NOTE: This component uses the Legacy Unity GUI and is not supported on mobile devices" +
			"\nThis component allows the user to modify objects of a given tag in the scene. This powerful functionality adds transform manipulation" +
			" directly into your game, great for user-created objects and levels." +
			"\n\n" +
			"To use, add this component to an object in your scene and set the 'Ray Mask' you wish to allow the player to edit");

		void Start () {
			mode[0] = "Move";
			mode[1] = "Rotate";
			mode[2] = "Scale";
			axis[0] = "X";
			axis[1] = "Y";
			axis[2] = "Z";
			axis[3] = "W";
			space[0] = "World";
			space[1] = "Self";
		}

		void Update () {
			if (Input.GetMouseButtonDown(mouseButton)) {
				InitiateTransformByMouse();
			}
		}
		
		void OnGUI () {
			GUI.skin = guiSkin;
			if (target != null)
				GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), TransformGUI, "Transform Tools");
		}

		void TransformGUI (int id) {
			if (target != null) {
				currentMode = GUILayout.Toolbar(currentMode, mode, GUILayout.ExpandHeight(true));
				currentAxis = GUILayout.Toolbar(currentAxis, axis, GUILayout.ExpandHeight(true));
				if (currentMode != 2)
					currentSpace = GUILayout.Toolbar(currentSpace, space, GUILayout.ExpandHeight(true));
				UpdateTransformerSettings ();
				GUILayout.BeginVertical("","");
//				GUILayout.Label("Snap to: ");
//				GUILayout.BeginHorizontal();
//				if(GUILayout.Button("X"))
//					target.SendMessage("SnapToSpecificGrid",Vector3.right * gridSize);
//				if(GUILayout.Button("Y"))
//					target.SendMessage("SnapToSpecificGrid",Vector3.up * gridSize);
//				if(GUILayout.Button("Z"))
//					target.SendMessage("SnapToSpecificGrid",Vector3.forward * gridSize);
//				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Grid Size:");
				gridSize = System.Convert.ToSingle( GUILayout.TextField(gridSize.ToString()));
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				if (GUILayout.Button("Done")) {
					Destroy(transformer);
					target = null;
				}
			}
		}

		public void InitiateTransformByMouse () {
			RaycastHit hinfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hinfo, Mathf.Infinity, (int)rayMask)) {
				target = hinfo.collider.gameObject;
				InitiateTransform(target);
			}
		}
		
		void InitiateTransform (GameObject _target) {
			if (transformer != null)
				Destroy(transformer);
			transformer = target.AddComponent<Transformer>();
			transformer.updateMode = Transformer.UpdateModes.Screen;
		}
		
		void UpdateTransformerSettings () {
			if (currentMode == 0)
				transformer.transformationType = Transformer.TransformationTypes.Position;
			if (currentMode == 1)
				transformer.transformationType = Transformer.TransformationTypes.Rotation;
			if (currentMode == 2)
				transformer.transformationType = Transformer.TransformationTypes.Scale;

			if (currentAxis == 0)
				transformer.directionality = Transformer.Directionalities.X;
			if (currentAxis == 1)
				transformer.directionality = Transformer.Directionalities.Y;
			if (currentAxis == 2)
				transformer.directionality = Transformer.Directionalities.Z;
			if (currentAxis == 3)
				transformer.directionality = Transformer.Directionalities.W;
			if (transformer.directionality == Transformer.Directionalities.W && transformer.transformationType != Transformer.TransformationTypes.Scale)
				transformer.directionality = Transformer.Directionalities.Z;

			if (currentSpace == 0)
				transformer.transformationSpace = Transformer.TransformationSpaces.World;
			if (currentSpace == 1)
				transformer.transformationSpace = Transformer.TransformationSpaces.Self;
		}
	}
}
//Copyright 2014 William Hendrickson
