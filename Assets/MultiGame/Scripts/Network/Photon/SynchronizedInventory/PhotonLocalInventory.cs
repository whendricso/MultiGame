using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Inventory/Photon Local Inventory")]
	public class PhotonLocalInventory : PhotonModule {

		public static PhotonLocalInventory localInventory;
		public string filePath = "Inventory";
		public Rect screenArea;
		public GUISkin guiSkin;
		public GameObject instantiationTransform;
		[ReorderableAttribute]
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

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Local Inventory stores inventory data in a binary file on the player's machine, and when they instantiate " +
			"an inventory item, MultiGame ensures that the other clients instantiate the same one correctly. Place on the Player prefab for persistent, Photon-sychronized inventory." +
			"Takes a list of Equip Slots representing the transforms where we instantiate and parent equipment which corresponds to the 'Usage Slot' for each Photon Pickable Item." +
			"\n\n" +
			"To create a new inventory item, first create two copies of the same model in the Scene View, add a 'Photon Pickable Item' to the first and a 'Photon Active Item' to the second. " +
			"The 'Photon Active Item' should now have components added to it for any particle effects or game behavior you want it to have. Finally, place this object in a folder named " +
			"'Resources' anywhere in your Project View. Adjust the settings on the 'Photon Pickable Item' and place it next to the other prefab in the Project View as well.");

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
			if (transform.root.gameObject.GetComponent<PhotonView>().isMine) {
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

			PhotonActiveItem activeItem = inGameObject.GetComponent<PhotonActiveItem>();
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
			if (!transform.root.gameObject.GetComponent<PhotonView>().isMine)
				return;
			if (debug)
				Debug.Log("Saving inventory");

			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream;

			Requisition();

			try {
				stream = File.Open(filePath, FileMode.Create);
				formatter.Serialize(stream, items);
			}
			catch {
				return;
			}
		}

		public IEnumerator Load () {
			if (debug)
				Debug.Log("Load invoked, inventory view is mine: " + transform.root.GetComponent<PhotonView>().isMine);
			if (transform.root.GetComponent<PhotonView>().isMine) {




				BinaryFormatter formatter = new BinaryFormatter();
				FileStream stream;
				try {
					stream = File.Open(Application.persistentDataPath + "/" + filePath, FileMode.Open);
					using (stream) {
						items = formatter.Deserialize(stream) as List<InventoryItem>;
					}
				}
				catch {
					Debug.Log("Loading inventory file failed. Perhaps it does not yet exist?");
				}

				Requisition();
				

			}
			else
				yield return null;
		}

	}
}