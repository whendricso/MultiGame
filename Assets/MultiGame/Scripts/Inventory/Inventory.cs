﻿ using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Inventory/Inventory")]
	public class Inventory : MultiModule {
		
		//TODO:Inventory system needs a major update
		[Header("Heirarchy Settings")]
		[RequiredField("Right hand transform, should be an empty object parented to the hand and rotated to match the rotation of any items you wish the character to hold")]
		public GameObject rWeaponMount;
		[RequiredField("Left hand transform, should be an empty object parented to the hand and rotated to match the rotation of any items you wish the character to hold")]
		public GameObject lWeaponMount;
		[RequiredField("Torso transform, should be an empty object parented to the torso and rotated to match the rotation of any items you wish the character to wear")]
		public GameObject torsoMount;
		[RequiredField("Back transform, should be an empty object parented to the back and rotated to match the rotation of any items you wish the character to wear")]
		public GameObject backMount;
		[RequiredField("A transform generally in front of the character where items which are instantiated rather than equipped will appear. Used mostly for single-use items.")]
		public GameObject instantiationTransform;//Where do we instantiate objects that are "no equip" type?

		[Header("Inventory Settings")]
		[Tooltip("Max items in inventory")]
		public int inventorySize = 10;
		[HideInInspector]
		public float buttonPad = 5.0f;

		[RequiredField("File name to store this inventory under")]
		public string fileName = "inv";

		[Header("Input Settings")]
		[Tooltip("Should we lock the cursor when closing the inventory, and unlock it when opening the inventory?")]
		public bool controlCursorState = false;
		[Tooltip("Key allowing the player to open/close the inventory GUI")]
		public KeyCode inventoryKey = KeyCode.I;
		[Tooltip("Can the first items be accessed using number keys? This iterates through the inventory in-order looking for anything that is set to 'Weapon R' mount (right-handed weapon).")]
		public bool allowNumberKeys = true;
		public KeyCode nextWeapon = KeyCode.Q;
		public KeyCode previousWeapon = KeyCode.None;
		[Tooltip("How sensitive is the mouse wheel when switching weapons?")]
		public float weaponSwapSensitivity = 0.025f;

		[Header("GUI Settings")]
		[Tooltip("Should we show the GUI displaying the inventory contents?")]
		public bool showInventoryGUI = false;
		[Tooltip("Should we use buttons to drop/stow items held in the hand?")]
		public bool useHandButtons = true;
		public GUISkin guiSkin;
		[Tooltip("Normalized viewport rectangle indicating the inventory GUI area")]
		public Rect inventoryArea = new Rect(.2f,.2f,.6f,.6f);
		[Tooltip("How many buttons can we have in a row?")]
		public int numButtonsPerRow = 4;
		[Tooltip("How many buttons can we have in a column?")]
		public int numButtonsPerColumn = 4;



		//Local members
		//		[Tooltip("Items already in the inventory")]
		/// <summary>
		/// The Player's static inventory stored as key-vaue pairs. This is only the player's inventory, other objects must use a Container instead.
		/// </summary>
		public static Dictionary<string, GameObject> inv = new Dictionary<string, GameObject>();
		//		[Tooltip("number of each remaining")]
		public static Dictionary<string, int> invCount = new Dictionary<string, int>();
		/// <summary>
		/// The containers thaat are currently opened by the player, meaning that they are displaying their contents on screen. Used for dropping items into containers
		/// </summary>
		public static List<Container> openContainers = new List<Container>();
		private string lastKey;
		private GameObject lastWeapon;

		public HelpInfo help = new HelpInfo("Does not use object pooling. This component implements a locally-saved single-player only inventory for player items. It's static, meaning there can only be one at any given time," +
			" or weird stuff might happen (probably not, but no guarantees). Each inventory item needs 2 prefabs: an 'ActiveItem' which represents the object while it's in-use," +
			" as well as a 'Pickable' which represents the item when it's on the ground. These need to have references to each other in the inspector and they all need to be in a" +
			"'Resources' folder or you might get errors when saving & loading.");

		public struct InventoryData {
			public Dictionary<string, int> invData;

			public InventoryData (Dictionary<string,int> _data) {
				invData = _data;
			}
		}
		
		void Update () {
			if (Input.GetKeyUp(inventoryKey)) {
				showInventoryGUI = !showInventoryGUI;
				if (controlCursorState) {
					if (showInventoryGUI)
						CursorLock.SetLock(false);
					else
						CursorLock.SetLock(true);
				}
			}
			if (Input.GetKeyDown(nextWeapon) || Input.GetAxis("Mouse ScrollWheel") > weaponSwapSensitivity)
				NextWeapon(false);
			if (Input.GetKeyDown(previousWeapon) || Input.GetAxis("Mouse ScrollWheel") < -weaponSwapSensitivity)
				NextWeapon(true);
			if (allowNumberKeys)
				ProcessNumberKeys();
		}

		[Header("Available Messages")]
		public MessageHelp saveHelp = new MessageHelp("Save","Saves the inventory in a binary file on the player's machine. Does not work on web builds");
		public void Save () {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);

			Dictionary<string, int> _data = new Dictionary<string, int>();

			foreach (KeyValuePair<string, GameObject> _kvp in inv) {
				_data.Add(_kvp.Value.name, invCount[_kvp.Key]);
			}

			formatter.Serialize(stream, _data);
			stream.Close();
			StartCoroutine(SaveEquip());
		}

		private IEnumerator SaveEquip() {
			yield return new WaitForEndOfFrame();
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + "equip", FileMode.Create);

			Dictionary<string, string> _data = new Dictionary<string, string>();

			ActiveObject rWeapon = rWeaponMount.GetComponentInChildren<ActiveObject>();
			ActiveObject lWeapon = lWeaponMount.GetComponentInChildren<ActiveObject>();
			ActiveObject torso = torsoMount.GetComponentInChildren<ActiveObject>();
			ActiveObject back = backMount.GetComponentInChildren<ActiveObject>();

			_data.Add("rWeapon", rWeapon == null ? "null" : rWeapon.gameObject.name);
			_data.Add("lWeapon", lWeapon == null ? "null" : lWeapon.gameObject.name);
			_data.Add("torso", torso == null ? "null" : torso.gameObject.name);
			_data.Add("back", back == null ? "null" : back.gameObject.name);

			foreach (string str in _data.Values)
				Debug.Log(str);

			formatter.Serialize(stream, _data);
			stream.Close();
		}

		public MessageHelp loadHelp = new MessageHelp("Load","Loads inventory from a binary file on the player's machine. Does not work on web builds");
		public void Load () {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream;
			try {
				stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
			}
			catch {
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
			StartCoroutine(LoadEquip());
		}

		private IEnumerator LoadEquip() {
			yield return new WaitForEndOfFrame();
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream;

			//try {
				stream = File.Open(Application.persistentDataPath + "/" + fileName + "equip", FileMode.Open);
			//} catch {
				//return;
			//}

			ActiveObject rWeapon = rWeaponMount.GetComponentInChildren<ActiveObject>();
			ActiveObject lWeapon = lWeaponMount.GetComponentInChildren<ActiveObject>();
			ActiveObject torso = torsoMount.GetComponentInChildren<ActiveObject>();
			ActiveObject back = backMount.GetComponentInChildren<ActiveObject>();

			if (rWeapon != null)
				Destroy(rWeapon.gameObject);
			if (lWeapon != null)
				Destroy(lWeapon.gameObject);
			if (torso != null)
				Destroy(torso.gameObject);
			if (back != null)
				Destroy(torso.gameObject);

			Dictionary<string, string> _data = new Dictionary<string, string>(formatter.Deserialize(stream) as Dictionary<string,string>);


			GameObject rw = null;
			GameObject lw = null;
			GameObject ts = null;
			GameObject bk = null;

			if (_data["rWeapon"] != "null")
				rw = Resources.Load(_data["rWeapon"]) as GameObject;
			if (_data["lWeapon"] != "null")
				lw = Resources.Load(_data["lWeapon"]) as GameObject;
			if (_data["torso"] != "null")
				ts = Resources.Load(_data["torso"]) as GameObject;
			if (_data["back"] != "null")
				bk = Resources.Load(_data["back"]) as GameObject;

			foreach (string str in _data.Values)
				Debug.Log(str);

			if (rw != null) {
				rw = Instantiate(rw) as GameObject;
				MountObject(rw, rWeaponMount);
			}
			if (lw != null) {
				lw = Instantiate(lw) as GameObject;
				MountObject(lw, lWeaponMount);
			}
			if (ts != null) {
				ts = Instantiate(ts) as GameObject;
				MountObject(ts, torsoMount);
			}
			if (bk != null) {
				bk = Instantiate(bk) as GameObject;
				MountObject(bk, backMount);
			}
			stream.Close();
		}

		void OnGUI () {
			if (!showInventoryGUI)
				return;
			if (guiSkin != null)
				GUI.skin = guiSkin;
			
			if (useHandButtons) {//drop and stow buttons for equip slots
				GUILayout.BeginArea(new Rect(inventoryArea.x * Screen.width, (inventoryArea.y - (inventoryArea.height / 3)) * Screen.height, (inventoryArea.width /* 3*/) * Screen.width, (inventoryArea.height / 3) * Screen.height));
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical("","box");
				GUILayout.Label("L Hand");
				if(GUILayout.Button("Drop"))
					lWeaponMount.BroadcastMessage("Drop", SendMessageOptions.DontRequireReceiver);
				if(GUILayout.Button("Stow"))
					lWeaponMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
				GUILayout.EndVertical();
				GUILayout.BeginVertical("","box");
				GUILayout.Label("R Hand");
				if(GUILayout.Button("Drop"))
					rWeaponMount.BroadcastMessage("Drop", SendMessageOptions.DontRequireReceiver);
				if(GUILayout.Button("Stow"))
					rWeaponMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
				GUILayout.EndVertical();

				GUILayout.BeginVertical("", "box");
				GUILayout.Label("Back");
				if (GUILayout.Button("Drop"))
					backMount.BroadcastMessage("Drop", SendMessageOptions.DontRequireReceiver);
				if (GUILayout.Button("Stow"))
					backMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
				GUILayout.EndVertical(); GUILayout.BeginVertical("", "box");
				GUILayout.Label("Armor");
				if (GUILayout.Button("Drop"))
					torsoMount.BroadcastMessage("Drop", SendMessageOptions.DontRequireReceiver);
				if (GUILayout.Button("Stow"))
					torsoMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
				GUILayout.EndVertical();

				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}

			GUILayout.BeginArea(new Rect(inventoryArea.x * Screen.width, inventoryArea.y * Screen.height, inventoryArea.width * Screen.width, inventoryArea.height * Screen.height),"","box");
			//GUILayout.Box("Inventory");
			int currentButton = 0;
			GUILayout.BeginHorizontal();
			foreach (KeyValuePair<string, GameObject> kvp in inv) {
				currentButton ++;
				if (currentButton > numButtonsPerRow) {
					GUILayout.EndHorizontal();
					currentButton = 0;
					GUILayout.BeginHorizontal();
				}
				GUIContent btn;
				ActiveObject activeItem = kvp.Value.GetComponent<ActiveObject>();
				if (activeItem.icon != null) {
					btn = new GUIContent(activeItem.icon,kvp.Key);
				}
				else
					btn = new GUIContent(kvp.Key);
				GUILayout.BeginVertical();
				if(GUILayout.Button(btn,GUILayout.Height(-buttonPad + ((inventoryArea.height * Screen.height)) /numButtonsPerColumn),GUILayout.Width(-buttonPad + ((inventoryArea.width * Screen.width))/numButtonsPerRow))) {
					if (Input.GetMouseButtonUp(1)) {//right-clicked
						Container con = GetOpenContainer();
						if (con == null) {
							Instantiate(kvp.Value.GetComponent<ActiveObject>().pickable, instantiationTransform.transform.position, instantiationTransform.transform.rotation);
							Remove(kvp.Key);
						} else {
							if (!con.inv.ContainsKey(kvp.Key)) {
								con.inv.Add(kvp.Key, kvp.Value);
								con.invCount.Add(kvp.Key, 1);
							} else {
								if (con.invCount.ContainsKey(kvp.Key))
									con.invCount[kvp.Key]++;
								else
									con.invCount.Add(kvp.Key, 1);
							}
							Remove(kvp.Key);
						}
						return;
					}
					else {
						GameObject activeObj = Instantiate(inv[kvp.Key]) as GameObject;
						ActiveObject activ = activeObj.GetComponent<ActiveObject>();
						switch (activ.itemType) {
						case ActiveObject.ItemTypes.WeaponR:
							rWeaponMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
							MountObject(activ.gameObject, rWeaponMount);
							break;
						case ActiveObject.ItemTypes.WeaponL:
							lWeaponMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
							MountObject(activ.gameObject, lWeaponMount);
							break;
						case ActiveObject.ItemTypes.EquipTorso:
							torsoMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
							MountObject(activ.gameObject, torsoMount);
							break;
						case ActiveObject.ItemTypes.EquipBack:
							backMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
							MountObject(activ.gameObject, backMount);
							break;
						case ActiveObject.ItemTypes.NoEquip:
							activ.transform.position = instantiationTransform.transform.position;
							activ.transform.rotation = instantiationTransform.transform.rotation;
							break;
						}
						if (activ.closeInventoryOnSelect)
							showInventoryGUI = false;
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

		/// <summary>
		/// Find containers which are currently opened by the player
		/// </summary>
		/// <returns>Returns a reference to a Container which is both open and has a free slot available. Returns null if none are found.</returns>
		Container GetOpenContainer() {
			Container ret = null;

			if (openContainers.Count > 0) {
				foreach (Container con in openContainers) {
					if (con.inv.Count < con.maxItems)
						ret = con;
					break;
				}
			}

			return ret;
		}

		void MountObject (GameObject activeObj, GameObject mount) {
			//if (mount.transform.childCount > 0)
			//	activeObj.SendMessage("Stow");
			activeObj.AddComponent<CloneFlagRemover>();
			activeObj.transform.parent = mount.transform;
			activeObj.transform.localPosition = Vector3.zero;
			activeObj.transform.localRotation = Quaternion.identity;
		}
		
		public bool Pick(KeyValuePair<string, GameObject> kvp) {
			bool ret = false;
			if (inv.Count < inventorySize) {
				ret = true;
				if(!(inv.ContainsKey(kvp.Key))) {
					inv.Add(kvp.Key, kvp.Value);
					if (invCount.ContainsKey(kvp.Key))
						invCount[kvp.Key]++;
					else
						invCount.Add(kvp.Key, 1);
				}
				else {
					if (invCount.ContainsKey(kvp.Key))
						invCount[kvp.Key]++;
					else
						invCount.Add(kvp.Key, 1);
				}
			}
			return ret;
		}

		public MessageHelp removeHelp = new MessageHelp("Remove","Allows you to remove an item from the player's inventory by passing it's inventory key",4,"The inventory key of the item you want to remove");
		public void Remove (string key) {
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

		public MessageHelp nextWeaponHelp = new MessageHelp("NextWeapon","Searches the inventory for the next Active Object with 'Item Type' of 'WeaponR' and equips it",
			1,"Should we reverse the operation (previous weapon)?");
		public void NextWeapon (bool reverse) {
			if (rWeaponMount.transform.childCount == 0) {
				if (reverse) {
					GameObject last = GetLastGun();
					if (last != null) {
						GameObject gun = Instantiate(last) as GameObject;
						MountObject(gun, rWeaponMount);
						Remove(gun.GetComponent<ActiveObject>().inventoryKey);
					}
				}
				else {
					foreach (GameObject gobj in inv.Values) {
						ActiveObject activ = gobj.GetComponent<ActiveObject>();
						if (activ.itemType == ActiveObject.ItemTypes.WeaponR) {
							GameObject gun = Instantiate(gobj) as GameObject;
							MountObject(gun, rWeaponMount);
							Remove(activ.inventoryKey);
							return;
						}
						
					}
				}
			}
			else {
				//if (rWeaponMount.transform.GetChild(0) != null && rWeaponMount.transform.GetChild(0).gameObject.GetComponent<ActiveObject>() != null)
				try {
					lastKey = rWeaponMount.transform.GetChild(0).gameObject.GetComponent<ActiveObject>().inventoryKey;
				} catch {
					Debug.LogError("Inventory " + gameObject.name + " requires that 'R Weapon Mount', 'L Weapon Mount', 'Torso', and 'Back' transforms should have 0 children! Parent an " +
					"empty object to the selected transform or bone, and use that for the equipment slot instead.");
				}
				//else
				//	lastKey = "";
				rWeaponMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
				bool foundIt = false;
				GameObject prev = GetPreviousGun(lastKey);
				if (!reverse) {
					foreach (GameObject gobj in inv.Values) {
						ActiveObject activ = gobj.GetComponent<ActiveObject>();
						if (activ.itemType == ActiveObject.ItemTypes.WeaponR && foundIt) {
							GameObject gun = Instantiate(gobj) as GameObject;
							MountObject(gun, rWeaponMount);
							Remove(activ.inventoryKey);
							return;
						}
						if (gobj.GetComponent<ActiveObject>().inventoryKey == lastKey)
							foundIt = true;
					}
				}
				else {
					if (prev == null)
						return;
					GameObject gun = Instantiate(prev) as GameObject;
					MountObject(gun, rWeaponMount);
					Remove(gun.GetComponent<ActiveObject>().inventoryKey);
					return;
				}
			}
		}
		
		private GameObject GetPreviousGun(string currentKey) {
			if (string.IsNullOrEmpty(currentKey))
				return null;
			string previousKey = "";
			GameObject previous = null;
			foreach (GameObject gobj in inv.Values) {
				ActiveObject activ = gobj.GetComponent<ActiveObject>();
				if (activ.itemType == ActiveObject.ItemTypes.WeaponR) {
					if (activ.inventoryKey == currentKey) {
						if (inv.ContainsKey(previousKey))
							previous = inv[previousKey];
					}
					else
						previousKey = activ.inventoryKey;
				}
			}
			return previous;
		}
		
		private GameObject GetLastGun() {
			GameObject last = null;
			foreach (GameObject gobj in inv.Values) {
				ActiveObject activ = gobj.GetComponent<ActiveObject>();
				if (activ.itemType == ActiveObject.ItemTypes.WeaponR) {
					last = gobj;
				}
			}
			return last;
		}
		
		private void GunByNumber (int number) {
			number--;//offset by -1 to avoid counting errors
			rWeaponMount.BroadcastMessage("Stow", SendMessageOptions.DontRequireReceiver);
			GameObject ret = null;
			List<GameObject> guns = new List<GameObject>();
			foreach (GameObject gobj in inv.Values) {
				ActiveObject activ = gobj.GetComponent<ActiveObject>();
				if (activ.itemType == ActiveObject.ItemTypes.WeaponR)
					guns.Add(gobj);
			}
			if (guns.Count >= number+1)
				ret = guns[number];
			
			if (ret != null) {
				GameObject gun = Instantiate(ret) as GameObject;
				MountObject(gun, rWeaponMount);
				Remove(gun.GetComponent<ActiveObject>().inventoryKey);
			}
			
		}
		
		private void ProcessNumberKeys () {
			if (Input.GetKeyDown(KeyCode.Alpha1))
				GunByNumber(1);
			if (Input.GetKeyDown(KeyCode.Alpha2))
				GunByNumber(2);
			if (Input.GetKeyDown(KeyCode.Alpha3))
				GunByNumber(3);
			if (Input.GetKeyDown(KeyCode.Alpha4))
				GunByNumber(4);
			if (Input.GetKeyDown(KeyCode.Alpha5))
				GunByNumber(5);
			if (Input.GetKeyDown(KeyCode.Alpha6))
				GunByNumber(6);
			if (Input.GetKeyDown(KeyCode.Alpha7))
				GunByNumber(7);
			if (Input.GetKeyDown(KeyCode.Alpha8))
				GunByNumber(8);
			if (Input.GetKeyDown(KeyCode.Alpha9))
				GunByNumber(9);
		}
	}
}