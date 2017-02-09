using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class VehicleEditor : MultiModule {

		[Tooltip("Used to delineate attachment node objects, which should have this tag. If you want the nodes to turn on/off automatically when building, add a MessageToggle component to them.")]
		public string nodeTag;
		public List<GameObject> modules = new List<GameObject>();
		public List<GameObject> attachables = new List<GameObject>();

		public KeyCode cancelKey = KeyCode.Mouse1;
		public KeyCode placementKey = KeyCode.Mouse0;

		public KeyCode increaseHeight = KeyCode.Q;
		public KeyCode decreaseHeight = KeyCode.E;

		[Tooltip("Height difference between each Y level for the vehicle. So, if the height of a corridor prefab is 3 meters (including floor and ceiling) then this number should be 3.")]
		public float yLevelHeight = 3f;
		/// <summary>
		/// The current Y level is multiplied by yLevelHeight to give us the actual Y Offset of the placement plane
		/// </summary>
		private int currentYLevel = 0;
		/// <summary>
		/// The placement plane is an invisible collider which we raycast onto to get the final object placement
		/// </summary>
		private GameObject placementPlane;
		private Collider placementCollider;

		[HideInInspector]
		public GameObject vehicle;
		/// <summary>
		/// The selector indicates which module or attachable we currently have selected.
		/// </summary>
		private int selector = 0;
		private GameObject currentPlacement;

		public bool useGui = true;
		public Rect guiArea = new Rect(.71f,.31f,.28f,.28f);

		public enum Modalities {Modules, Attachables};
		public Modalities modality = Modalities.Modules;

		/// <summary>
		/// Placement active is true when we want to place an object on the vehicle.
		/// </summary>
		private bool placementActive = false;
		private Vector2 scrollArea;

		public class CustomCursor {
			public Texture2D texture;
			public Vector2 hotspot;
			public CursorMode mode;

			CustomCursor(Texture2D _texture, Vector2 _hotspot, CursorMode _mode) {
				texture = _texture;
				hotspot = _hotspot;
				mode = _mode;
			}
		}

		void OnGUI() {
			if (!useGui)
				return;
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");

			if (GUILayout.Button("Modules"))
				modality = Modalities.Modules;
			if (GUILayout.Button("Attachments"))
				modality = Modalities.Attachables;

			scrollArea = GUILayout.BeginScrollView(scrollArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			
			if (modality == Modalities.Modules) {
				for (int i = 0; i < modules.Count; i++) {
					if (GUILayout.Button(modules[i].name)) {
						selector = i;
						EnablePlacement();
					}
				}
			}
			if (modality == Modalities.Attachables) {
				for (int j = 0; j < attachables.Count; j++) {
					if (GUILayout.Button(attachables[j].name)) {
						selector = j;
						EnablePlacement();
					}
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		void Update () {
			if (Input.GetKeyDown(cancelKey))
				DisablePlacement();

			if (!placementActive)
				return;

			if (placementPlane != null) {
				RaycastHit hinfo;
				bool didHit = placementCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f);
				if (didHit) {
					placementPlane.transform.position = new Vector3(hinfo.point.x, (yLevelHeight * currentYLevel),hinfo.point.z);

				}
			}

		}

		public MessageHelp placeAgainHelp = new MessageHelp("PlaceAgain","Call this to repeat placement of the previous object without opening the GUI");
		public void PlaceAgain () {
			EnablePlacement();
		}

		private void EnablePlacement () {
			placementActive = true;
			placementPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
			placementPlane.transform.RotateAround(placementPlane.transform.position, Vector3.right, 90f);
			placementPlane.transform.localScale = new Vector3(100f,.1f,100f);
			placementPlane.GetComponent<MeshRenderer>().enabled = false;
			placementCollider = placementPlane.GetComponent<Collider>();
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOn", SendMessageOptions.DontRequireReceiver);
			}
		}
		private void DisablePlacement () {
			placementActive = false;
			if (placementPlane != null)
				Destroy(placementPlane);
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOff", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}