using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pickable. Object that can be clicked to pick up by local player.
/// Receives SendMessage("Pick", [Inventory]);
/// Optionally, supply the Inventory instance being used,
/// otherwise it will try to send the object to the Player's inventory.
/// </summary>
public class Pickable : MultiModule {
	
	[Tooltip("How far can we be from the object tagged 'Player' and still be pickable?")]
	public float pickRange = 2.4f;
	public enum PickModes {Item, Deployable/*, Character*/};
	[Tooltip("Are we picking up an inventory item, or a Deployable object?")]
	public PickModes pickMode = PickModes.Item;
	[Tooltip("A Game Object prefab with an Active Object component, matching this one with Inventory Key")]
	public GameObject activeObject;//the object to be instantiated when used
	[Tooltip("A unique key identifying this as a unique type of object")]
	public string inventoryKey;
	public bool debug = false;
	
	private bool picked  = false;

	public HelpInfo help = new HelpInfo("This component implements items that can be picked up and placed in inventory. To cause this to happen, send the 'Pick' message to" +
		" this object. Pickables need a corresponding ActiveObject which represents the item while it's in-use.");
	
	void Start () {
		switch (pickMode) {
		case PickModes.Item:
			if (inventoryKey == null) {
				Debug.LogError("Pickable " + gameObject.name + " requires an inventory key to be assigned in the inspector!");
				Destroy(gameObject);
				return;
			}
			if (activeObject == null) {
				Debug.LogError("Pickable " + gameObject.name + " requires an active object to be assigned in the inspector!");
				Destroy(gameObject);
				return;
			}
			break;
		case PickModes.Deployable:
			if (activeObject == null) {
				Debug.LogError("Pickable " + gameObject.name + " requires an active object to be assigned in the inspector!");
				Destroy(gameObject);
				return;
			}
			break;
		}
	}	
	
	void OnMouseUpAsButton() {
		Pick();
	}

	void OnCollisionExit (Collision collisionInfo) {
		if(collisionInfo.collider.gameObject.tag == "Player")
			picked = false;
	}
	
	public void Pick () {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null) {
			Debug.LogError("Pickable on " + gameObject.name + " could not find a Player object. Try sending message 'Pick' with the optional Inventory reference supplied.");
			return;
		}
		if (Vector3.Distance(transform.position, player.transform.position) > pickRange)
			return;
		Inventory inv = player.GetComponent<Inventory>();
		if (inv == null) {
			Debug.LogError("Pickable on " + gameObject.name + " could not find an Inventory component on the Player. Try sending message 'Pick' with the optional Inventory reference supplied.");
			return;
		}
		Pick(inv);
		if (debug)
			Debug.Log("Pick " + gameObject.name + " picked: " + picked);
	}
	
	public void Pick (Inventory inventory) {
		if (PlayerPrefs.HasKey(gameObject.name + "magazineCount")) {
			Debug.Log("Cleared ammo for " + gameObject.name);
			PlayerPrefs.DeleteKey(gameObject.name + "magazineCount");
		}
		if (picked)
			return;
		picked = true;
		switch (pickMode) {
		case PickModes.Item:
			KeyValuePair<string, GameObject> kvp = new KeyValuePair<string, GameObject>(inventoryKey, activeObject);
			if(inventory.Pick(kvp))
				Destroy(gameObject);
			break;
		case PickModes.Deployable:
			GameObject player = inventory.gameObject;
			Deployer deployer = player.GetComponent<Deployer>();
			if (deployer != null) {
				for (int i = 0; i < deployer.deployables.Length; i++ ) {
					if (deployer.deployables[i].name == activeObject.name && deployer.deployablesCount[i] < deployer.deployablesMax[i]) {
						if(debug)
							Debug.Log("Player " + player.name + " Pick success " + activeObject.name + " compared against " + deployer.deployables[i].name);
						player.SendMessage("PickDeployable", activeObject, SendMessageOptions.DontRequireReceiver);
						Destroy(gameObject);
					}
				}
				
			}
			else {
				if (debug) {
					Debug.LogError("Pick " + gameObject.name + " could not get a Deployer component! Pick failed!");
				}
			}
//			Debug.Log("Picked deployable: " + activeObject);
			break;
		}
	}
}
