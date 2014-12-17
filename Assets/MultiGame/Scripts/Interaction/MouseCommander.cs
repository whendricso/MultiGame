using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Mouse commander hndles RTS-style construction via mouse input.
/// </summary>
public class MouseCommander : MonoBehaviour {

	//public int windowID = 0;
	public bool useGUI = true;
	public Rect guiArea;
	public GUISkin guiSkin;

	public float deadZone = 0.25f;
	public Vector2 stickInput = Vector2.zero;
	public float force = 1000.0f;
	public float ySpeed = 20.0f;

	//cutscene mode disables movement handling until we're told to leave cutscene mode
	public enum Modes {Build, Command, Cutscene };
	public Modes mode = Modes.Command;
	public enum Layouts {Vertical, Horizontal };
	public Layouts layout = Layouts.Horizontal;

	[System.NonSerialized]
	public int currentSelection = -1;
	public int buttonWidth = 64;
	public int buttonHeight = 64;
	public Deployable[] deploys;
	[HideInInspector]
	public GameObject[] deployables;
	[HideInInspector]
	public Texture2D[] icons;
	[HideInInspector]
	public int[] quantities;
	[HideInInspector]
	public int[] maxQuantities;
	[System.NonSerialized]
	public Vector2 scrollPosition = Vector2.zero;

	[System.Serializable]
	public class Deployable {
		public GameObject deploy;
		public Texture2D icon;
		public int quantity = -1;
		public int maxQuantity = -1;
	}

	void Start () {
		if (deployables.Length > 0) {//Old deployer was set up in the editor already, this makes it backwards-compatibl
			if (!CheckForMatchingInfo()) {
				Debug.LogError("Mouse Commander " + gameObject.name + " needs matching information for each deployable including icons, quantities, and max quantities. If no icon is used, simply enter nothing and the prefab's name will be used instead.");
				enabled = false;
				return;
			}
		}
		if (camera != null) {
			if (rigidbody == null)
				gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = false;
			if (rigidbody.drag <= 0.0f)
				rigidbody.drag = force / 100;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
		}
	}

	void OnGUI () {
		if (guiSkin != null)
			GUI.skin = guiSkin;
		if (!useGUI || mode != Modes.Build)//should we show the building GUI?
			return;
		//GUILayout.Window (windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),GUIWin, "Build");

	//}
		GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height));//,"","box");
	//void GUIWin (int index) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition/*, GUILayout.Width(guiArea.width * Screen.width), GUILayout.Height(guiArea.height * Screen.height)*/);
		#region beginGUI
		if (layout == Layouts.Horizontal)
			GUILayout.BeginHorizontal();
		else
			GUILayout.BeginVertical();
		#endregion
		
		for (int i = 0; i < deploys.Length; i++) {
			GUILayout.BeginVertical();
			if (deploys[i].icon == null) {
				if (GUILayout.Button(deploys[i].deploy.gameObject.name, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					currentSelection = i;//Deploy(i);
				if (deploys[i].maxQuantity > 0)
					GUILayout.Label("" + deploys[i].quantity + "/" + deploys[i].maxQuantity);
			}
			else {
				if (GUILayout.Button(deploys[i].icon, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					currentSelection = i;//Deploy(i);
				if (deploys[i].maxQuantity > 0)
					GUILayout.Label("" + deploys[i].quantity + "/" + deploys[i].maxQuantity);
			}
			GUILayout.EndVertical();
		}
		
		#region endGUI
		if (layout == Layouts.Horizontal)
			GUILayout.EndHorizontal();
		else
			GUILayout.EndVertical();
		#endregion
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
	
	void Update () {
		if ((mode != Modes.Cutscene) && (camera != null))
			UpdateCam();
		if (Input.GetMouseButtonDown(0) && currentSelection != -1) {
			Deploy(currentSelection);
			currentSelection = -1;
		}
	}

	void UpdateCam () {
		//smooth deadzone without loss of precision
		#region stickInput
		stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if(stickInput.magnitude < deadZone)
			stickInput = Vector2.zero;
		else
			stickInput = stickInput.normalized * ((stickInput.magnitude - deadZone) / (1 - deadZone));
		#endregion

		rigidbody.AddRelativeForce(stickInput.x * force, 0.0f, stickInput.y * force, ForceMode.Force);
		transform.Translate(0.0f, Input.GetAxis("Mouse ScrollWheel") * ySpeed, 0.0f);

	}

	void Deploy (int selector ) {
		//Debug.Log("Deploy " + selector);
		if (deploys.Length >= selector)//deployables[selector] == null)
			return;
		RaycastHit hinfo;
		Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool didHit = Physics.Raycast(_ray, out hinfo);

		if (!didHit)
			return;
		if (deploys[selector].maxQuantity > -1 && deploys[selector].quantity <= 0 )
			return;

		if (deploys[selector].deploy.rigidbody != null)//deployables[selector].rigidbody != null)
			Instantiate(deploys[selector].deploy, new Vector3(hinfo.point.x, hinfo.point.y + deploys[selector].deploy.renderer.bounds.extents.y / 2 + 0.02f,hinfo.point.z), Quaternion.identity);//deployables[selector], new Vector3(hinfo.point.x, hinfo.point.y + deployables[selector].renderer.bounds.extents.y / 2 + 0.02f,hinfo.point.z), Quaternion.identity);
		else
			Instantiate(deploys[selector].deploy, hinfo.point, Quaternion.identity);//deployables[selector], hinfo.point, Quaternion.identity);
		deploys[selector].quantity --;

	}

	/// <summary>
	/// Checks to make sure all the arrays have the same length, one entry for each deployable.
	/// </summary>
	/// <returns><c>true</c>, if info was matching, <c>false</c> otherwise.</returns>
	public bool CheckForMatchingInfo () {
		bool ret = true;

		if (deployables.Length < 1)
			ret = false;
		if (icons.Length != deployables.Length)
			ret = false;
		if ( quantities.Length != deployables.Length)
			ret = false;
		if (maxQuantities.Length != deployables.Length)
			ret = false;

		return ret;
	}
}
//Copyright 2014 William Hendrickson
