using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class VehicleEditor : MultiModule {

		[Tooltip("Used to delineate attachment node objects, which should have this tag. If you want the nodes to turn on/off automatically when building, add a MessageToggle component to them.")]
		public string nodeTag;
		[ReorderableAttribute]
		public List<GameObject> modules = new List<GameObject>();
		[Tooltip("The collision layers which the modules deploy ray collides with. These usually include the modules themselves as well as any ground plane or background geometry, for best results.")]
		public LayerMask moduleMask;
		[ReorderableAttribute]
		public List<GameObject> attachables = new List<GameObject>();
		[Tooltip("The collision mask for objects we can actually deploy attachables onto, this represents surfaces on the inside or outside of the vehicle where things can be attached.")]
		public LayerMask attachableMask;

		public float snapDistance = 16f;

		public KeyCode cancelKey = KeyCode.Mouse1;
		public KeyCode placementKey = KeyCode.Mouse0;

//		public KeyCode increaseHeight = KeyCode.Q;
//		public KeyCode decreaseHeight = KeyCode.E;

		[Tooltip("The root object of the vehicle, if undefined, this will be the first section placed.")]
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

		public bool debug = false;

		/// <summary>
		/// Placement active is true when we want to place an object on the vehicle.
		/// </summary>
		private bool placementActive = false;
		private Vector2 scrollArea;

		private RaycastHit hinfo;

//		public class CustomCursor {
//			public Texture2D texture;
//			public Vector2 hotspot;
//			public CursorMode mode;
//
//			CustomCursor(Texture2D _texture, Vector2 _hotspot, CursorMode _mode) {
//				texture = _texture;
//				hotspot = _hotspot;
//				mode = _mode;
//			}
//		}

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

			if (Input.GetKeyDown(placementKey) && currentPlacement != null) {
				List<GameObject> snapNodes = new List<GameObject>();
				for (int i = 0; i < currentPlacement.transform.childCount; i++) {
					if (currentPlacement.transform.GetChild(i).tag == nodeTag)
						snapNodes.Add(currentPlacement.transform.GetChild(i).gameObject);
				}
				foreach( GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
					for (int j = 0; j < snapNodes.Count; j++) {
						if (Vector3.Distance( Camera.main.WorldToScreenPoint(gobj.transform.position) , Camera.main.WorldToScreenPoint(snapNodes[j].transform.position) ) < snapDistance) {
							SnapTo(gobj, snapNodes[j]);
						}
					}
				}
			}

			if (currentPlacement != null) {
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f, moduleMask, QueryTriggerInteraction.Ignore)) {
					currentPlacement.transform.position = hinfo.point;
				}


			}
		}

		private void SnapTo (GameObject baseNode, GameObject targetNode) {
			if (debug)
				Debug.Log("Vehicle Editor " + gameObject.name + " is snapping " + targetNode.transform.root.gameObject.name + " to " + baseNode.name);

			targetNode.transform.root.position = (baseNode.transform.parent.position + baseNode.transform.localPosition) - targetNode.transform.localPosition;
		}

		public MessageHelp placeAgainHelp = new MessageHelp("PlaceAgain","Call this to repeat placement of the previous object without opening the GUI");
		public void PlaceAgain () {
			EnablePlacement();
		}

		private void EnablePlacement () {
			placementActive = true;
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOn", SendMessageOptions.DontRequireReceiver);
			}

			if (modality == Modalities.Modules) {
				currentPlacement = Instantiate(modules[selector], transform.position, transform.rotation);
				if (vehicle == null) {
					vehicle = currentPlacement;
					placementActive = false;//stop placement if we're placing the root object
				}
				else
					MessageManager.Send(new MessageManager.ManagedMessage(currentPlacement,"ToggleOn", MessageManager.ManagedMessage.SendMessageTypes.Broadcast,null,MessageManager.ManagedMessage.ParameterModeTypes.None));


			}
		}
		private void DisablePlacement () {
			placementActive = false;
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOff", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}