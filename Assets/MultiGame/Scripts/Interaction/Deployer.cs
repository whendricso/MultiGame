using UnityEngine;
using System.Collections;

//Deployer should be placed on a player's base object that will deploy the object

public class Deployer : MonoBehaviour {
	
	#region members
	public bool useDeployColor = true;
	public string forbiddenSurfaceTag = "NoDeploy";
	public bool showGUI = true;
	public Rect guiArea;
	public Rect deployButton;
	
	public GameObject[] deployables;
	private GameObject deployedItem;
	public GameObject[] ghostDeployables;//the "ghost" or hologram-type image to show during the deploy process
	public Texture2D[] buttons;
	public int[] deployablesCount;
	public int[] deployablesMax;
	public enum Directions {Horizontal, Vertical};
	public Directions direction = Directions.Horizontal;
	public float buttonWidth = 64.0f;
	public float buttonHeight = 64.0f;
	
	private GameObject ghost;
	[HideInInspector]
	public int currentItem = 0;
	public GameObject deployRayOrigin;
	public float deployRayRange = 2.4f;//cast straight down from the position of the deployRayOrigin
	[HideInInspector]
	public bool deploying = false;
	public KeyCode deployModeKey = KeyCode.Q;
	public KeyCode nextItem = KeyCode.X;
	public KeyCode previousItem = KeyCode.Z;
	public KeyCode deployItem = KeyCode.E;
	private bool canDeploy = false;
	
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
			if (ghost != null && ghost.renderer != null) {
				if (canDeploy)
					ghost.renderer.material.color = Color.green;
				else
					ghost.renderer.material.color = Color.red;
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