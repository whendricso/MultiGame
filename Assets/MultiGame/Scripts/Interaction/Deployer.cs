using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

//Deployer should be placed on a player's base object that will deploy the object

	[AddComponentMenu("MultiGame/Interaction/Deployer")]
	public class Deployer : MultiModule {
		
		#region members
		[Tooltip("Should we automatically set the deploy ghost red/green depending on if it can be deployed? (recommended)")]
		public bool useDeployColor = true;
		[RequiredFieldAttribute("Forbidden objects should have this tag so you can't deploy on them")]
		public string forbiddenSurfaceTag = "NoDeploy";
		[Tooltip("Should we use a legacy Unity GUI? Not suitable for mobile devices")]
		public bool showGUI = true;
		[Tooltip("Normalized viewport rectangle indicating the screen area for the legacy GUI, values between 0 and 1")]
		public Rect guiArea = new Rect(0.01f, 0.01f, 0.88f, 0.1f);
		[Tooltip("Normalized viewport rectangle indicating the screen area for the 'Deploy' button, values between 0 and 1")]
		public Rect deployButton = new Rect(0.89f, 0.01f, 0.2f, 0.1f);
		
		[Tooltip("Ordered list of objects that can be deployed")]
		public GameObject[] deployables;
		private GameObject deployedItem;
		[Tooltip("Ordered list matching the Deployables list indicating prefabs with no collider used as 'ghosts' or 'holograms' showing where the object will go")]
		public GameObject[] ghostDeployables;//the "ghost" or hologram-type image to show during the deploy process
		[Tooltip("Ordered list matching the Deployables list indicating the button textures")]
		public Texture2D[] buttons;
		[Tooltip("Ordered list showing the available number of each deployable currently available")]
		public int[] deployablesCount;
		[Tooltip("Ordered list showing the max available for each deployable")]
		public int[] deployablesMax;
		public enum Directions {Horizontal, Vertical};
		[Tooltip("Direction to render the button list")]
		public Directions direction = Directions.Horizontal;
		public float buttonWidth = 64.0f;
		public float buttonHeight = 64.0f;
		
		private GameObject ghost;
		[HideInInspector]
		public int currentItem = 0;
		[RequiredFieldAttribute("An object indicating the origin of the deploy ray. In a first person game, should be in front of and slightly above the camera. Raycasts downward automatically")]
		public GameObject deployRayOrigin;
		[RequiredFieldAttribute("How far from the Deploy Ray Origin do we look down to check if we can deploy on a given surface?")]
		public float deployRayRange = 2.4f;//cast straight down from the position of the deployRayOrigin
		[HideInInspector]
		public bool deploying = false;
		[Tooltip("Key for turning on/off deploy mode")]
		public KeyCode deployModeKey = KeyCode.Q;
		[Tooltip("Key for scrolling through available deploys")]
		public KeyCode nextItem = KeyCode.X;
		[Tooltip("Key for scrolling back through available deploys")]
		public KeyCode previousItem = KeyCode.Z;
		[Tooltip("Key that deploys the selected item immediately")]
		public KeyCode deployItem = KeyCode.E;
		private bool canDeploy = false;

		public HelpInfo help = new HelpInfo("This component implements TF2-style deployment functionality. To use, supply a matching list for each object in 'Deployables' you need one in 'Ghost Deployables' " +
			"'Buttons' 'Deployables Count' and 'Deployables Max'. So for example if 'Deployables' #3 is a turret, then 'Deployables Count' #3 indicates how many turrets we have." +
			"\n\n" +
			"For an in-depth explanation of use, see the accompanying documentation file (found in this folder)");
		
		public bool debug = false;
		#endregion
		
		void Start () {
			
			#region errorHandling
			if (deployables.Length < 1) {
				Debug.LogError("Deployable needs a prefab list to create!");
				enabled = false;
				return;
			}
			if (deployRayOrigin == null) 
				deployRayOrigin = transform.Find("DeployRayOrigin").gameObject;
			if (deployRayOrigin == null) {
				Debug.LogError("Deployable needs a 'DeployRayOrigin' game object to indicate the position the deployable's ray should be cast downward from.");
				enabled = false;
				return;
			}
				
			if (ghostDeployables.Length != deployables.Length) {
				Debug.LogError("Deployables and Ghost Deployables must have the same number of items, one ghost for each deployable it represents");
				enabled = false;
				return;
			}
			if (deployables.Length != buttons.Length) {
				Debug.LogError("Deployables and Buttons must have the same number of items, one ghost for each deployable it represents");
				enabled = false;
				return;
			}
			if (deployablesCount.Length != deployables.Length) {
				Debug.LogError("Deployables must have a Deployables count for each deployable type, to indicate the number remaining");
				enabled = false;
				return;
			}
			if (deployablesMax.Length != deployables.Length) {
				Debug.LogError("Deployables must have a Deployables Max count for each deployable type, to indicate the number remaining");
				enabled = false;
				return;
			}
			#endregion
		}
		
		void OnGUI () {
			if(!showGUI)
				return;
			if(!deploying)
				return;
			if( GUI.Button(new Rect(deployButton.x * Screen.width, deployButton.y * Screen.height, deployButton.width * Screen.width, deployButton.height * Screen.height), "Deploy"))
				Deploy();
			GUILayout.BeginArea(new Rect( guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height));
			if (direction == Directions.Horizontal)
				GUILayout.BeginHorizontal();
			else
				GUILayout.BeginVertical();
			for (int i = 0; i < deployables.Length; i++) {
				GUILayout.BeginVertical();
				if(GUILayout.Button(buttons[i], GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
					currentItem = i;
					if(ghost != null)
						Destroy(ghost);
				}
				GUILayout.Label("" + deployablesCount[i] + "/ " + deployablesMax[i]);
				GUILayout.EndVertical();
			}
			if (direction == Directions.Horizontal)
				GUILayout.EndHorizontal();
			else
				GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		
		void Update () {
			
			#region Input
			if (Input.GetKeyDown( nextItem)) {//cycle deployables
				currentItem += 1;
				if (currentItem >= deployables.Length)
					currentItem = 0;
				if (ghost != null)
					Destroy(ghost);
			}
			if (Input.GetKeyDown( previousItem)) {
				currentItem -= 1;
				if (currentItem < 0)
					currentItem = deployables.Length - 1;
				if (ghost != null)
					Destroy(ghost);
			}
			if (Input.GetKeyDown( deployModeKey))//deploy toggle
				deploying = !deploying;
			
			#endregion
			
			#region objectPlacement
			if (!deploying) {
				if (ghost != null)
					Destroy(ghost);
				return;
			}
			if (ghost == null)
				ghost = Instantiate(ghostDeployables[currentItem]) as GameObject;
			else
				ghost.transform.rotation = Quaternion.identity;
			RaycastHit hinfo;
			if (Physics.Raycast(deployRayOrigin.transform.position, Vector3.down, out hinfo, deployRayRange)) {
				if (Input.GetKeyDown( deployItem) && deploying)
					Deploy (hinfo);
				ghost.transform.position = hinfo.point;
				ghost.transform.rotation = transform.rotation;
				if (hinfo.collider.gameObject.tag == forbiddenSurfaceTag)
					canDeploy = false;
				else
					canDeploy = true;
			}
			else {
				canDeploy = false;
				ghost.transform.position = new Vector3( deployRayOrigin.transform.position.x, deployRayOrigin.transform.position.y - deployRayRange, deployRayOrigin.transform.position.z);
			}
			if (useDeployColor) {
				if (ghost != null && ghost.GetComponent<Renderer>() != null) {
					if (canDeploy)
						ghost.GetComponent<Renderer>().material.color = Color.green;
					else
						ghost.GetComponent<Renderer>().material.color = Color.red;
				}
			}
		}
		
		public void Deploy () {
			if (canDeploy && deployablesCount[currentItem] > 0) {
				deployablesCount[currentItem]--;
				Instantiate(deployables[currentItem], ghost.transform.position, ghost.transform.rotation);
				deploying = false;
			}
		}
		
		public void Deploy (RaycastHit hinfo) {
			if (canDeploy && deployablesCount[currentItem] > 0) {
				deployablesCount[currentItem]--;
				Instantiate(deployables[currentItem], hinfo.point, transform.rotation);
				deploying = false;
			}
		}
		
		public void PickDeployable (GameObject deploy) {
			for (int i = 0; i < deployables.Length; i++) {
				if (deploy.name == deployables[i].name) {
					if (debug)
						Debug.Log("Picked deployable: " + deployables[i].name + " with " + deployablesCount[i] + " remaining of" + deployablesMax[i]);
					if (deployablesCount[i] < deployablesMax[i])
						deployablesCount[i] += 1;
				}
			}
		}
			#endregion
	}
}