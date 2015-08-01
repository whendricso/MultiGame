using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[RequireComponent(typeof(PhotonView))]
public class CloudInventory : Photon.MonoBehaviour {

	public static CloudInventory localInventory;

	public Rect screenArea;
	public GUISkin guiSkin;
	public GameObject instantiationTransform;
	public List<GameObject> equipSlots = new List<GameObject>();
	public int maxItems = 10;
	public bool automatic = true;
	public string collectibleObjectTag = "Collectible";
	public string activeObjectTag = "ActiveObject";



	[HideInInspector]
	public List<InventoryItem> items = new List<InventoryItem>();
	private List<InventoryItem> currentlyActiveItems = new List<InventoryItem>();
	public KeyCode inventoryButton = KeyCode.I;
	[System.NonSerialized]
	public bool showInventory = false;
	[HideInInspector]
	public Vector2 scrollPosition = new Vector2(0f,0f);

	public bool debug = false;

	[System.Serializable]
	public class InventoryItem {
		public string name = "";
		public string collectiblePrefab = "";
		public string activePrefab = "";
		public int quantity = 1;
		public int usageSlot = 0;

		public InventoryItem(string _name, string _collectiblePrefab, string _activePrefab, int _quantity, int _usageSlot) {
			name = _name;
			collectiblePrefab = _collectiblePrefab;
			activePrefab = _activePrefab;
			quantity = _quantity;
			usageSlot = _usageSlot;
		}
	}

	void Start () {
		if (photonView.isMine) {
			localInventory = this;
			if(debug)
				Debug.Log ("View is local");
		}
		for (int j = 0; j < equipSlots.Count; j++) {
			//TODO: rewrite this line for Amazon Web Services
			//equipSlots[j].name = "" + ParseUser.CurrentUser.Username + j;
		}
	//	StartCoroutine(Load());
	}

	void OnGUI () {
		if (!showInventory)
			return;

		if(guiSkin != null)
			GUI.skin = guiSkin;


		GUILayout.BeginArea(new Rect(screenArea.x * Screen.width, screenArea.y * Screen.height, screenArea.width * Screen.width, screenArea.height * Screen.height),"Inventory","box");
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		GUILayout.Label("Capacity: " + items.Count + "/" + maxItems);
		for (int i = 0; i < items.Count; i++) {
			if (GUILayout.Button(items[i].name, GUILayout.ExpandWidth(true))) {
				if (Input.GetMouseButtonUp(1)) {
					Drop(i);
					return;
				}
				else {
					Use(i);
					return;
				}
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void Update () {
		if (Input.GetKeyUp(inventoryButton))
			showInventory = !showInventory;
	}

	void OnEnable () {
		if(automatic)
			StartCoroutine(Load());
	}

	void OnDisable () {
		Requisition();
	}

	void OnDestroy () {
		Requisition();
	}

	void Requisition () {
		items.AddRange(currentlyActiveItems);
		currentlyActiveItems.Clear();
		Save();
	}

	public void ShowInventory () {
		showInventory = true;
	}

	public void HideInventory () {
		showInventory = false;
	}

	public void Use (int item) {
//		if (items.Count >= item) {
//			Debug.LogError("Parse Inventory cannot use item [" + item + "] because it does not exist!");
//			return;
//		}

		equipSlots[items[item].usageSlot].BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
		GameObject inGameObject = PhotonNetwork.Instantiate(items[item].activePrefab, equipSlots[items[item].usageSlot].transform.position,equipSlots[items[item].usageSlot].transform.rotation,0);
		//TODO: rewrite this line for AWS
//		inGameObject.SendMessage("Attach", ("" + ParseUser.CurrentUser.Username + items[item].usageSlot), SendMessageOptions.DontRequireReceiver);

		CloudActiveItem activeItem = inGameObject.GetComponent<CloudActiveItem>();
		activeItem.itemName = items[item].name;
		activeItem.collectiblePrefab = items[item].collectiblePrefab;
		activeItem.usageSlot = items[item].usageSlot;
		activeItem.quantity = items[item].quantity;
		
		currentlyActiveItems.Add(items[item]);
		items.RemoveAt(item);
		Save();

	}

	public void Drop (int item) {
		if (instantiationTransform == null)
			PhotonNetwork.Instantiate(items[item].collectiblePrefab, transform.position, transform.rotation,0);
		else
			PhotonNetwork.Instantiate(items[item].collectiblePrefab, instantiationTransform.transform.position, instantiationTransform.transform.rotation,0);
		items.RemoveAt(item);
		Save();
	}

	public void Save () {
		if (!photonView.isMine)
			return;
		if (debug)
			Debug.Log("Saving inventory");
		using(MemoryStream stream = new MemoryStream()) {
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, items);
			//TODO: rewrite this serialization code for AWS
//			ParseFile pFile = new ParseFile("Inventory" + ParseUser.CurrentUser.Username,stream);
//			pFile.SaveAsync().ContinueWith(task => {
//				if(!task.IsCanceled && !task.IsFaulted) {
//					ParseUser.CurrentUser["InventoryURL"] = pFile.Url;
//					ParseUser.CurrentUser.SaveAsync();
//				}
//			});
		}
	}

	public IEnumerator Load () {
		if (debug)
			Debug.Log("Load invoked, inventory view is mine: " + photonView.isMine);
		if (photonView.isMine) {
			//TODO: rewrite this serialization code for AWS
			//ParseFile pFile = ParseUser.CurrentUser.Get<ParseFile>("Inventory" + ParseUser.CurrentUser.Username);
//			string url = ParseUser.CurrentUser.Get<string>("InventoryURL");
//			WWW inventoryFileRequest = new WWW( url);
//			while(!inventoryFileRequest.isDone)
//				yield return null;
//			using (MemoryStream stream = new MemoryStream( inventoryFileRequest.bytes)) {
//				BinaryFormatter formatter = new BinaryFormatter();
//				items = (List<InventoryItem>)formatter.Deserialize(stream);
//			}
		}
		else
			yield return null;
	}
}