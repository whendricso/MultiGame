using UnityEngine;
using System.Collections;

public class ClickPainter : MonoBehaviour {

	public Rect guiArea;
	public int mouseButton = 1;
	public Material[] materials;
	public string[] paintableTags;

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
//Copyright 2014 William Hendrickson
