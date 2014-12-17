﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class Inventory : MonoBehaviour {
	
	public static Dictionary<string, GameObject> inv = new Dictionary<string, GameObject>();
	public static Dictionary<string, int> invCount = new Dictionary<string, int>();
	public bool useHandButtons = true;
	public string inventoryPath = "Inventory.xml";
	public GameObject rWeaponMount;
	public GameObject lWeaponMount;
	public GameObject torsoMount;
	public GameObject backMount;
	public GameObject instantiationTransform;//Where do we instantiate objects that are "no equip" type?
	public int inventorySize = 10;
	public int numButtonsPerRow = 4;
	public int numButtonsPerColumn = 4;
	[HideInInspector]
	public float buttonPad = 5.0f;
	public bool showInventoryGUI = false;
	public KeyCode inventoryKey = KeyCode.I;
	public GUISkin guiSkin;
	public Rect inventoryArea;
	public bool allowNumberKeys = true;
	public KeyCode nextWeapon = KeyCode.Q;
	public KeyCode previousWeapon = KeyCode.None;
	public float weaponSwapSensitivity = 0.25f;
	private GameObject lastWeapon;
	private string lastKey;
	
	void Start () {
		
//		inventoryPath = Path.Combine(Application.persistentDataPath, "Inventory.xml");
//		Inventory loaded = Load(inventoryPath);
//		if (loaded == null)
//			return;
//		inv.Clear();
//		foreach (GameObject activ in loaded.inv.Values) {
//			ActiveObject activObj = activ.GetComponent<ActiveObject>();
//			inv.Add(activObj.inventoryKey, activ);
//		}
	}
	
	void Update () {
		if (Input.GetKeyUp(inventoryKey)) {
			showInventoryGUI = !showInventoryGUI;
			if (showInventoryGUI)
				Screen.lockCursor = false;
			else
				Screen.lockCursor = true;
		}
		if (Input.GetKeyDown(nextWeapon) || Input.GetAxis("Mouse ScrollWheel") > weaponSwapSensitivity)
			NextWeapon(false);
		if (Input.GetKeyDown(previousWeapon) || Input.GetAxis("Mouse ScrollWheel") < -weaponSwapSensitivity)
			NextWeapon(true);
		if (allowNumberKeys)
			ProcessNumberKeys();
	}
	
	void OnGUI () {
		if (!showInventoryGUI)
			return;
		if (guiSkin != null)
			GUI.skin = guiSkin;
		
		if (useHandButtons) {//drop and stow buttons for the hands. Especially important for the left hand.
			GUILayout.BeginArea(new Rect(inventoryArea.x * Screen.width, (inventoryArea.y - (inventoryArea.height / 3)) * Screen.height, (inventoryArea.width / 3) * Screen.width, (inventoryArea.height / 3) * Screen.height));
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
					Instantiate(kvp.Value.GetComponent<ActiveObject>().pickable, instantiationTransform.transform.position, instantiationTransform.transform.rotation);
					Remove(kvp.Key);
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
	
	void MountObject (GameObject activeObj, GameObject mount) {
		if (mount.transform.childCount > 0)
			activeObj.SendMessage("Stow");
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
			lastKey = rWeaponMount.transform.GetChild(0).gameObject.GetComponent<ActiveObject>().inventoryKey;
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
	
	void OnDestroy () {
		
	}
}
