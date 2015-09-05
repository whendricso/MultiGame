using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//inventory-enabled object
//such as an item or weapon that is currently in-use
//plays an audio clip, if one is on the object
public class ActiveObject : MultiModule {
	
	public enum ItemTypes {WeaponR, WeaponL, EquipTorso, EquipBack, NoEquip};
	[Tooltip("Type of item based on equip slot")]
	public ItemTypes itemType = ItemTypes.NoEquip;
	[Tooltip("Icon for this object in inventory")]
	public Texture2D icon;
	public float iconSizeX = 32.0f;
	public float iconSizeY = 32.0f;
	[Tooltip("Unique name of this inventory item, matching it's 'Pickable' mate")]
	public string inventoryKey;
	[Tooltip("The pickable object associated with this item")]
	public GameObject pickable;
	public bool debug = false;
	[Tooltip("Close inventory GUI automatically when selecting an item?")]
	public bool closeInventoryOnSelect = true;
//	public float weight = 1.0f;

	public HelpInfo help = new HelpInfo("This component represents an inventory item that is currently in-use. Generally, this is something attached to the player like a " +
		"+5 Helmet of the Dunce or a machine gun. It must have a 'Pickable' prefab associated with it that has a 'Pickable' component on it and both must be inside a " +
		"'Resources' folder, otherwise you will get an error.");
	
	void Start () {
		#region errorHandling
		if (string.IsNullOrEmpty( inventoryKey)) {
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