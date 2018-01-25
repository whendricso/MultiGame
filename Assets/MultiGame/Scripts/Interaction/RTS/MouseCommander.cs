using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {


	/// <summary>
	/// Mouse commander hndles RTS-style construction via mouse input.
	/// </summary>
	[AddComponentMenu("MultiGame/Interaction/Mouse Commander")]
	public class MouseCommander : MultiModule {

		[Header("Important - Must be Populated")]
		[Tooltip("What objects can we deploy on, by collision mask?")]
		public LayerMask deployMask;
		//public int windowID = 0;

		[Header("GUI Settings")]
		[Tooltip("Should we use Unity's legacy GUI for this? Not suitable for mobile devices")]
		public bool useGUI = true;
		public enum Layouts {Vertical, Horizontal };
		[Tooltip("Which direction should the buttons be drawn in?")]
		public Layouts layout = Layouts.Horizontal;
		[Tooltip("Normalized viewport rectangle indicating where we should draw the buttons. Numbers  indicate a percentage of screen space from 0 to 1")]
		public Rect guiArea = new Rect(0.01f, 0.8f, .98f, .79f);
		public GUISkin guiSkin;
		public int buttonWidth = 64;
		public int buttonHeight = 64;

		[Header("Input and Motion")]
		[Tooltip("Should we control the position of this object using a rigidbody? Useful for command cameras")]
		public bool controlPosition = true;
		[RequiredFieldAttribute("Stick dead zone, for control smoothing")]
		public float deadZone = 0.25f;
		[HideInInspector]
		public Vector2 stickInput = Vector2.zero;
		[Tooltip("Movement force to apply")]
		public float force = 1000.0f;
		[Tooltip("How fast does the scroll wheel move us?")]
		public float ySpeed = 20.0f;
		[Tooltip("What key, if any, can the player hold down to keep deploying more of the same thing?")]
		public KeyCode continuationModifier = KeyCode.LeftShift;

		[Header("Deploys and Resources")]
		[RequiredFieldAttribute("Tag of objects we can't deploy close to", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string radiusSearchTag = "";//tag to check against for deploy radius constraint
		[Tooltip("What objects can the player buy?")]
		[ReorderableAttribute]
		public Deployable[] deploys;
		[Tooltip("What resources, if any, exist in the game?")]//[ReorderableField()]//TODO: Finish reorderable fields
		[ReorderableAttribute]
		public List<ResourceManager.GameResource> resources = new List<ResourceManager.GameResource>();

		[Header("Message Senders")]
		[Tooltip("Sent when we can't afford something")]
		public MessageManager.ManagedMessage insufficientResourceMessage;
		[Tooltip("Sent when we can afford a selection")]
		public MessageManager.ManagedMessage itemSelectedMessage;
		[Tooltip("Sent when we failed to deploy something due to radius restriction")]
		public MessageManager.ManagedMessage itemTooCloseMessage;

		[HideInInspector]
		public GameObject[] deployables;// these purely exist for backwards-compatibility
		[HideInInspector]
		public Texture2D[] icons;
		[HideInInspector]
		public int[] quantities;
		[HideInInspector]
		public int[] maxQuantities;

		//some local variables
		[System.NonSerialized]
		public int currentSelection = -1;
		[System.NonSerialized]
		public Vector2 scrollPosition = Vector2.zero;
		[System.NonSerialized]//TODO: fully implement this!
		public float populationMultiplier = 1f;
		[System.NonSerialized]
		public Rigidbody body;
		[System.NonSerialized]
		Camera cam;
		//cutscene mode disables movement handling until we're told to leave cutscene mode
		public enum Modes {Build, Command, Cutscene };
		[HideInInspector]//this is probably just bloat
		public Modes mode = Modes.Build;

		public HelpInfo help = new HelpInfo("This component implements RTS-style camera motion and object deployment. Legacy GUI is not recommended for mobile. To use, add this " +
			"to your MainCamera (which must be tagged 'MainCamera') and then set up a list of deployable objects. This works with the 'ResourceManager' component (optionally) to " +
			"allow for a resource-based experience." +
			"\n\n" +
			"There is a more in-depth manual 'MouseCommanderDoc.rtf' found in the MultiGame/Scripts/Interaction/RTS folder in your project, explaining the system in more detail than can fit in this text box.");

		[Tooltip("Send useful information to the console")]
		public bool debug = false;

		[System.Serializable]
		public class Deployable {
			public GameObject deploy;
			public Vector3 offset;
			public Texture2D icon;
			public int quantity = 0;
			public int maxQuantity = 0;
			public float cost = 0;
			public float minimumDeployRadius = 3.0f;
			public string resourceUsed = "";
			[System.NonSerialized]
			public List<GameObject> deployed = new List<GameObject>();
		}

		void Awake () {
			ResourceManager.resources.AddRange(resources);
			if (deployMask == -1) {
				Debug.LogError("Mouse Commander " + gameObject.name + " does not have a deploy mask assigned in the inspector so deploying will be impossible!");
			}
		}

		void Start () {
			if (insufficientResourceMessage.target == null)
				insufficientResourceMessage.target = gameObject;
			if (itemSelectedMessage.target == null)
				itemSelectedMessage.target = gameObject;
			if (itemTooCloseMessage.target == null)
				itemTooCloseMessage.target = gameObject;

			cam = GetComponent<Camera>();
			body = GetComponent<Rigidbody>();
			if (deployables.Length > 0) {//Old deployer was set up in the editor already, this makes it backwards-compatibl
				if (!CheckForMatchingInfo()) {
					Debug.LogError("Mouse Commander " + gameObject.name + " needs matching information for each deployable including icons, quantities, and max quantities. If no icon is used, simply enter nothing and the prefab's name will be used instead.");
					enabled = false;
					return;
				}
			}
			if (cam != null && controlPosition) {
				if (body == null)
					body = gameObject.AddComponent<Rigidbody>();
				body.useGravity = false;
				if (body.drag <= 0.0f)
					body.drag = force / 100;
				body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
			}
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref insufficientResourceMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref itemSelectedMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref itemTooCloseMessage, gameObject);
		}

		void OnGUI () {
			if (guiSkin != null)
				GUI.skin = guiSkin;
			if (!useGUI || mode != Modes.Build)//should we show the building GUI?
				return;
			//GUILayout.Window (windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),GUIWin, "Build");

		//}
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height));//,"","box");

			if (layout == Layouts.Horizontal)
				GUILayout.BeginHorizontal();
			else
				GUILayout.BeginVertical();
			foreach (ResourceManager.GameResource resource in ResourceManager.resources) {
				//if (resource.limit > -1)
				GUILayout.Label("" + resource.resourceName + ": " + resource.quantity + "/" + resource.limit);
			}
			if (layout == Layouts.Horizontal)
				GUILayout.EndHorizontal();
			else
				GUILayout.EndVertical();

			//GUILayout.Label("Test");

		//void GUIWin (int index) {
			scrollPosition = GUILayout.BeginScrollView(scrollPosition/*, GUILayout.Width(guiArea.width * Screen.width), GUILayout.Height(guiArea.height * Screen.height)*/);
			#region beginGUI

			if (layout == Layouts.Horizontal)
				GUILayout.BeginHorizontal();
			else
				GUILayout.BeginVertical();
			#endregion

			for (int i = 0; i < deploys.Length; i++) {
				if (layout == Layouts.Horizontal)
					GUILayout.BeginVertical();
				else
					GUILayout.BeginHorizontal();

				GUILayout.FlexibleSpace();

				if (deploys[i].icon == null) {

					if (deploys[i].maxQuantity > 0)
						GUILayout.Label("" + deploys[i].quantity + " / " + Mathf.FloorToInt( deploys[i].maxQuantity * populationMultiplier));
					if (deploys[i].cost > 0)
						GUILayout.Label("" + deploys[i].cost);
					if (GUILayout.Button("[" + "" + (i + 1) + "] " + deploys[i].deploy.gameObject.name, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
						SelectDeploy(i);
					}
				}
				else {

					if (deploys[i].maxQuantity > 0)
						GUILayout.Label("" + deploys[i].quantity + "/" + deploys[i].maxQuantity);
					if (deploys[i].cost > 0)
						GUILayout.Label("" + deploys[i].cost);
					if (i < 9)
						GUILayout.Label("[" + (i + 1) + "]");
					if (GUILayout.Button(deploys[i].icon, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
						SelectDeploy(i);
				}
				if (layout == Layouts.Horizontal)
					GUILayout.EndVertical();
				else
					GUILayout.EndHorizontal();
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
		
		void FixedUpdate () {
			int _num = SelectByNumber();
			if (_num != -1)
				SelectDeploy( _num-1);
			if ((mode != Modes.Cutscene) && (cam != null))
				UpdateCam();
			if (Input.GetMouseButtonDown(0) && currentSelection != -1) {
				if (debug)
					Debug.Log("Deploying currently selected object " + currentSelection);
				Deploy(currentSelection);
				if(!Input.GetKey(continuationModifier))
					currentSelection = -1;
			}

			foreach (Deployable _deploy in deploys) {
				for (int i = 0; i < _deploy.deployed.Count; i++) {
					if (_deploy.deployed[i] == null) {
						_deploy.deployed.RemoveAt(i);
						_deploy.quantity--;
					}
				}
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

			if (body != null && controlPosition) {
				body.AddRelativeForce(stickInput.x * force, 0.0f, stickInput.y * force, ForceMode.Force);
				//TODO:should I add a raycheck on this line?
				transform.Translate(0.0f, Input.GetAxis("Mouse ScrollWheel") * ySpeed, 0.0f);
			}

		}

		[Header("Available Messages")]
		public MessageHelp selectDeployHelp = new MessageHelp("SelectDeploy","Activate deployment for a 'Deployable'", 2, "Index indicating which of the 'Deploys' you wish to allow the player to place '");
		public void SelectDeploy (int _selector) {
			currentSelection = _selector;//Deploy(i);
			if (deploys[_selector].cost > 0 && deploys[_selector].cost > ResourceManager.GetQuantityByName(deploys[_selector].resourceUsed)) {
				if (debug)
					Debug.Log("Mouse Commanger " + gameObject.name + " has insufficient resources for " + deploys[_selector].deploy.name);
				if (!string.IsNullOrEmpty( insufficientResourceMessage.message))
					MessageManager.Send( insufficientResourceMessage);
				currentSelection = -1;
				return;
			}
			MessageManager.Send(itemSelectedMessage);
		}

		void Deploy (int selector ) {
			if (deploys.Length <= selector)//deployables[selector] == null)
				return;

			if(debug)
				Debug.Log ("Deploy called with selector " + selector + " and deploy length " + deploys.Length);

			RaycastHit hinfo;
			Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			bool didHit = Physics.Raycast(_ray, out hinfo,Mathf.Infinity, deployMask);

			if (!didHit)
				return;

			if (debug)
				Debug.Log ("Clicked " + hinfo.collider.gameObject.name);

			if (deploys[selector].maxQuantity > 0 && deploys[selector].deployed.Count >= Mathf.FloorToInt( deploys[selector].maxQuantity * populationMultiplier)) {
				if (debug)
					Debug.Log("Max quantity of " + deploys[selector].deploy.name + " reached");
				return;
			}

			if (deploys[selector].cost > 0 && deploys[selector].cost > ResourceManager.GetQuantityByName(deploys[selector].resourceUsed)) {
				if (debug)
					Debug.Log("Not enough resources for" + deploys[selector].deploy.name);
			    return;
			}

			if (!string.IsNullOrEmpty( radiusSearchTag) && !CanDeployInRadius(selector,hinfo.point)) {
				if (!string.IsNullOrEmpty( itemTooCloseMessage.message))
					MessageManager.Send(itemTooCloseMessage);
				if (debug)
					Debug.Log("Cannot deploy " + deploys[selector].deploy.name + ", too close to another similar object.");
				return;
			}

			if (deploys[selector].cost > 0)
				ResourceManager.DeductQuantityByName(deploys[selector].resourceUsed, deploys[selector].cost);

			if(debug)
				Debug.Log("Deploying " + deploys[selector].deploy.name + " at position " + hinfo.point);

			GameObject _justDeployed;

			if (deploys[selector].deploy.GetComponent<Rigidbody>() != null && deploys[selector].deploy.GetComponent<Renderer>() != null)//deployables[selector].rigidbody != null)
				_justDeployed = Instantiate(deploys[selector].deploy, new Vector3(hinfo.point.x + deploys[selector].offset.x, hinfo.point.y + deploys[selector].deploy.GetComponent<Renderer>().bounds.extents.y / 2 + 0.02f + deploys[selector].offset.y,hinfo.point.z + deploys[selector].offset.z), Quaternion.identity) as GameObject;//deployables[selector], new Vector3(hinfo.point.x, hinfo.point.y + deployables[selector].renderer.bounds.extents.y / 2 + 0.02f,hinfo.point.z), Quaternion.identity);
			else
				_justDeployed = Instantiate(deploys[selector].deploy, new Vector3(hinfo.point.x + deploys[selector].offset.x, hinfo.point.y + deploys[selector].offset.y,hinfo.point.z + deploys[selector].offset.z), Quaternion.identity) as GameObject;//deployables[selector], hinfo.point, Quaternion.identity);
			deploys[selector].quantity ++;
			deploys[selector].deployed.Add(_justDeployed);

		}

		bool CanDeployInRadius (int _selector, Vector3 _position) {
			bool ret = true;

			List<GameObject> gobjs = new List<GameObject>();

			gobjs.AddRange(GameObject.FindGameObjectsWithTag(radiusSearchTag));
			foreach(GameObject gob in gobjs) {
				if (Vector3.Distance(_position, gob.transform.position) <= deploys[_selector].minimumDeployRadius) {
					if (debug)
						Debug.Log("Deploy " + deploys[_selector].deploy.name + " was too close to a " + gob.name);
					ret = false;
				}
			}

			return ret;
		}

		int SelectByNumber () {
			int ret = -1;

			if (Input.GetKeyDown(KeyCode.Alpha0))
				ret = 10;
			if (Input.GetKeyDown(KeyCode.Alpha1))
				ret = 1;
			if (Input.GetKeyDown(KeyCode.Alpha2))
				ret = 2;
			if (Input.GetKeyDown(KeyCode.Alpha3))
				ret = 3;
			if (Input.GetKeyDown(KeyCode.Alpha4))
				ret = 4;
			if (Input.GetKeyDown(KeyCode.Alpha5))
				ret = 5;
			if (Input.GetKeyDown(KeyCode.Alpha6))
				ret = 6;
			if (Input.GetKeyDown(KeyCode.Alpha7))
				ret = 7;
			if (Input.GetKeyDown(KeyCode.Alpha8))
				ret = 8;
			if (Input.GetKeyDown(KeyCode.Alpha9))
				ret = 9;

			return ret;
		}

		/// <summary>
		/// Checks to make sure all the arrays have the same length, one entry for each deployable.
		/// </summary>
		/// <returns><c>true</c>, if info was matching, <c>false</c> otherwise.</returns>
		bool CheckForMatchingInfo () {
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

		public MessageHelp toggleBuildGUIHelp = new MessageHelp("ToggleBuildGUI","Opens/closes the build GUI");
		public void ToggleBuildGUI () {
			useGUI = !useGUI;
		}

		void ToggleBuildGui (bool _enabled) {
			useGUI = _enabled;
		}

		public MessageHelp enableBuildGUIHelp = new MessageHelp("EnableBuildGUI","Opens the build GUI");
		public void EnableBuildGui () {
			ToggleBuildGui(true);
		}

		public MessageHelp disableBuildGUIHelp = new MessageHelp("DisableBuildGUI","Closes the build GUI");
		public void DisableBuildGui() {
			ToggleBuildGui(false);
		}

	}
}
//Copyright 2014 William Hendrickson
