using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MultiGame;

namespace MultiGame {

	public class Container : MultiModule {

		[Header("IMGUI Settings")]
		[Tooltip("What portion of the screen should the container occupy when opened?")]
		public Rect inventoryArea = new Rect(.01f, .01f, .2f, .2f);
		public bool showInventoryGUI = false;
		public GUISkin guiSkin;
		[Tooltip("How many buttons can we have in a row?")]
		public int numButtonsPerRow = 4;
		[Tooltip("How many buttons can we have in a column?")]
		public int numButtonsPerColumn = 4;
		[Header("Gameplay Settings")]
		public float maxDistance = 0;
		[RequiredField("The file name we wish to use to save & load the container's contents. Does not work on web.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string fileName = "";
		public int maxItems = 10;
		[Tooltip("Should the container have any items in it by default? Be sure to save and load the container if using this feature otherwise the contents will only appear the first time!!")]
		[Reorderable]
		public List<ActiveObject> items = new List<ActiveObject>();

		[HideInInspector]
		public float buttonPad = 5.0f;
		private GameObject player;
		Inventory inventory;
		private bool populated = false;

		public Dictionary<string, GameObject> inv = new Dictionary<string, GameObject>();
		public Dictionary<string, int> invCount = new Dictionary<string, int>();

		public HelpInfo help = new HelpInfo("Containers are objects which can hold items in a similar way to the Player's inventory. However, unlike the Inventory component, " +
			"you can have more than one container in your scene! To use, add this component to a chest, bag, or other object which you wish to hold items. Then, use a message sender to send the " +
			"'OpenContainer' message to this component. When the player right-clicks an object in their Inventory, it will be placed into the first available opened container. If you wish to " +
			"limit the accessable range of this container, assign the 'Max Distance' property above. The contents of the container can be serialized by passing the 'Save' and 'Load' messages " +
			"appropriately.");

		private void Start() {
			if (PlayerPrefs.HasKey("container" + fileName)) {
				if (PlayerPrefs.GetInt("container" + fileName) > 0)
					populated = true;
			}
			if (populated)
				return;
			populated = true;
			PlayerPrefs.SetInt("container" + fileName, 1);

			foreach (ActiveObject _active in items) {
				if (!inv.ContainsKey(_active.inventoryKey)) {
					inv.Add(_active.inventoryKey, _active.gameObject);
					invCount.Add(_active.inventoryKey, 1);
				} else {
					invCount[_active.inventoryKey]++;
				}
			}
		}

		private void Update() {
			if (maxDistance <= 0)
				return;
			if (player == null)
				player = GameObject.FindGameObjectWithTag("Player");
			if (player == null)
				return;
			if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
				CloseContainer();
		}

		private void OnGUI() {
			if (!showInventoryGUI)
				return;
			if (guiSkin != null)
				GUI.skin = guiSkin;

			GUILayout.BeginArea(new Rect(inventoryArea.x * Screen.width, inventoryArea.y * Screen.height, inventoryArea.width * Screen.width, inventoryArea.height * Screen.height), "", "box");

			int currentButton = 0;
			GUILayout.BeginHorizontal();

			foreach (KeyValuePair<string, GameObject> kvp in inv) {
				currentButton++;
				if (currentButton > numButtonsPerRow) {
					GUILayout.EndHorizontal();
					currentButton = 0;
					GUILayout.BeginHorizontal();
				}
				GUIContent btn;
				ActiveObject activeItem = kvp.Value.GetComponent<ActiveObject>();
				if (activeItem.icon != null) {
					btn = new GUIContent(activeItem.icon, kvp.Key);
				} else
					btn = new GUIContent(kvp.Key);
				GUILayout.BeginVertical();
				if (GUILayout.Button(btn, GUILayout.Height(-buttonPad + ((inventoryArea.height * Screen.height)) / numButtonsPerColumn), GUILayout.Width(-buttonPad + ((inventoryArea.width * Screen.width)) / numButtonsPerRow))) {
					if (Input.GetMouseButtonUp(1)) {//right-clicked
						return;//discard right mouse clicks for now (maybe use for crafting benches?
					} else {
						if (inventory == null && player != null)
							inventory = player.GetComponentInChildren<Inventory>();
						if (inventory == null)
							inventory = FindObjectOfType<Inventory>();
						if (inventory == null)
							return;
						inventory.Pick(kvp);
						Remove(kvp.Key);
						return;
					}
				}
				GUILayout.Label(invCount[kvp.Key] + " " + kvp.Key);
				GUILayout.EndVertical();
			}

			GUILayout.EndHorizontal();
			GUILayout.EndArea();

		}

		[Header("Available Messages")]
		public MessageHelp removeHelp = new MessageHelp("Remove", "Allows you to remove an item from the container by passing it's inventory key. This destroys the object.", 4, "The inventory key of the item you want to remove");
		public void Remove(string key) {
			if (invCount.ContainsKey(key)) {
				if (invCount[key] > 1)
					invCount[key] -= 1;
				else {
					invCount.Remove(key);
					if (inv.ContainsKey(key))
						inv.Remove(key);
				}
			}
		}

		public MessageHelp saveHelp = new MessageHelp("Save", "Saves the inventory in a binary file on the player's machine. Does not work on web builds");
		public void Save() {
			if (string.IsNullOrEmpty(fileName)) {
				Debug.LogError("Container " + gameObject.name + " reuires a filename for save & load functionality. Please specify a file name in the Inspector.");
				return;
			}
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);

			Dictionary<string, int> _data = new Dictionary<string, int>();

			foreach (KeyValuePair<string, GameObject> _kvp in inv) {
				//Debug.Log(_kvp.Key + " " + _kvp.Value.name + " " + inv.Count);
				_data.Add(_kvp.Value.name, invCount[_kvp.Key]);
			}

			formatter.Serialize(stream, _data);
			stream.Close();
		}

		public MessageHelp loadHelp = new MessageHelp("Load", "Loads inventory from a binary file on the player's machine. Does not work on web builds");
		public void Load() {
			if (string.IsNullOrEmpty(fileName)) {
				Debug.LogError("Container " + gameObject.name + " reuires a filename for save & load functionality. Please specify a file name in the Inspector.");
				return;
			}
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream;
			try {
				stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
			} catch {
				return;
			}

			inv.Clear();
			invCount.Clear();

			Dictionary<string, int> _data = new Dictionary<string, int>(formatter.Deserialize(stream) as Dictionary<string, int>);
			foreach (KeyValuePair<string, int> _kvp in _data) {
				GameObject _newEntry = Resources.Load(_kvp.Key) as GameObject;
				string _invKey = _newEntry.GetComponent<ActiveObject>().inventoryKey;
				inv.Add(_invKey, _newEntry);
				invCount.Add(_invKey, _kvp.Value);
			}
			stream.Close();
		}

		public MessageHelp openContainerHelp = new MessageHelp("OpenContainer","Opens the container GUI");
		public void OpenContainer() {
			Inventory.openContainers.Add(this);
			showInventoryGUI = true;
		}

		public MessageHelp closeContainerHelp = new MessageHelp("CloseContainer","Closes the container GUI");
		public void CloseContainer() {
			Inventory.openContainers.Remove(this);
			showInventoryGUI = false;
		}

		public MessageHelp toggleContainerHelp = new MessageHelp("ToggleContainer","Opens or closes the container based on context");
		public void ToggleContainer() {
			showInventoryGUI = !showInventoryGUI;
		}
	}
}