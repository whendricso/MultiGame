using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//inventory-enabled object
//such as an item or weapon that is currently in-use
//plays an audio clip, if one is on the object
public class ActiveObject : MonoBehaviour {
	
	public enum ItemTypes {WeaponR, WeaponL, EquipTorso, EquipBack, NoEquip};
	public ItemTypes itemType = ItemTypes.NoEquip;
	public Texture2D icon;
	public float iconSizeX = 32.0f;
	public float iconSizeY = 32.0f;
	public string inventoryKey;
	public GameObject pickable;
	public bool debug = false;
	public bool closeInventoryOnSelect = true;
	public float weight = 1.0f;
	
	void Start () {
		#region errorHandling
		if (inventoryKey == null) {
			Debug.LogError("Active Object " + gameObject.name + " requires an inventory key to be assigned in the inspector!");
			Destroy(gameObject);
			return;
		}
		if (pickable == null) {
			Debug.LogError("Active Object " + gameObject.name + " requires a pickable to be assigned in the inspector!");
			Destroy(gameObject);
			return;
		}
		#endregion
	}

	

	
	public void Stow () {//put the item back in inventory
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		Inventory inventory = player.GetComponent<Inventory>();
//		if (inventory.inv.ContainsKey(inventoryKey))
//			return;
//		inventory.inv.Add(inventoryKey, pickable.GetComponent<Pickable>().activeObject);
		KeyValuePair<string, GameObject> kvp = new KeyValuePair<string, GameObject>(inventoryKey, pickable.GetComponent<Pickable>().activeObject);
		inventory.Pick(kvp);
		if(debug)
			Debug.Log("Stowed pickable: " + pickable);
		if (GetComponent<AudioSource>() != null) {
			if (GetComponent<AudioSource>().clip != null)
				GetComponent<AudioSource>().Play();
		}
		Destroy(gameObject);
	}
	
	public void Drop () {
		if(debug)
			Debug.Log("Dropped pickable: " + pickable);
		if (GetComponent<AudioSource>() != null) {
			if (GetComponent<AudioSource>().clip != null)
				GetComponent<AudioSource>().Play();
		}
		Instantiate(pickable, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}