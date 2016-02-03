using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Click Painter")]
	public class ClickPainter : MultiModule {

		[Tooltip("Normalized viewport rectangle indicating the area reserved for palette selection")]
		public Rect guiArea = new Rect(0.75f, 0.1f, 3.49f, 2f);
		[Tooltip("Button used to click on objects in the scene to select which material to replace")]
		public int mouseButton = 1;
		[Tooltip("List of materials available as paint")]
		public Material[] materials;
		[Tooltip("List of tags of objects we can paint on")]
		public string[] paintableTags;
		[Tooltip("Should we use the Legacy Unity GUI?")]
		public bool useGUI = true;

	//	private int currentButton = 0;
		private int selector = 0;
		[HideInInspector]
		public bool painting = false;
		private GameObject paintingTarget = null;
		private Vector2 scrollArea = Vector2.zero;
	//	public int materialIndex = 0;
	//	public int previousIndex = 0;
	//	public Material previousMaterial;
	//	public GameObject previousObject;
		[HideInInspector]
		public Material[] _mats;

		public HelpInfo help = new HelpInfo("This component allows the player to change the material on objects. You need to provide a list of materials to choose from, and " +
			"a tag representing the objects that can be repainted. Legacy GUI is not recommended for mobile (in this case you must implement your own).");

		void OnGUI () {
			GUILayout.Window(101010,new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),MaterialWindow, "Paint Options");
		}

		void OnDisable () {
			painting = false;
		}

		void MaterialWindow (int id) {
			scrollArea = GUILayout.BeginScrollView(scrollArea, "box");
			if (!painting) {
				GUILayout.BeginVertical();
				int i = 0;
				for (; i < materials.Length; i ++) {
					if (GUILayout.Button(materials[i].name)) {
						selector = i;
						painting = true;
					}
				}
				GUILayout.EndVertical();
			}
			else {
				if(GUILayout.Button("Cancel")) {
					painting = false;
					paintingTarget = null;
				}
				GUILayout.Label("[Right Mouse]Material:\n" + this.materials[selector].name);
				if (paintingTarget != null) {
					_mats = paintingTarget.GetComponent<Renderer>().sharedMaterials;
					for (int i = 0; i < _mats.Length; i++) {
						if (GUILayout.Button("[" + _mats[i].name + "]", GUILayout.ExpandHeight(true))) {
							_mats[i] = this.materials[selector];
							paintingTarget.GetComponent<Renderer>().sharedMaterials = _mats;
						}
					}
				}

			}
			GUILayout.EndScrollView();
		}

		void Update () {
			if (!painting)
				return;


			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hinfo;
			if (Input.GetMouseButtonDown(mouseButton) && Physics.Raycast(ray, out hinfo)) {
				Debug.Log("Possibly painting " + hinfo.collider.gameObject.name);
				foreach(string paintableTag in paintableTags) {
					if (hinfo.collider.gameObject.tag == paintableTag) {

						//single material object
						if (hinfo.collider.GetComponent<Renderer>().sharedMaterials.Length == 1) {
							hinfo.collider.GetComponent<Renderer>().sharedMaterial = this.materials[selector];
							painting = false;
							paintingTarget = null;
							return;
						}

						paintingTarget = hinfo.collider.gameObject;

					}
				}
			}
		}


	}
}
//Copyright 2014 William Hendrickson
