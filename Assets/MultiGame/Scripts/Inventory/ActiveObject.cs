﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	//inventory-enabled object
	//such as an item or weapon that is currently in-use
	//plays an audio clip, if one is on the object
	[AddComponentMenu("MultiGame/Inventory/Active Object")]
	public class ActiveObject : MultiModule {

		public enum ItemTypes {WeaponR, WeaponL, EquipTorso, EquipBack, NoEquip};
		[Tooltip("Type of item based on equip slot. No Equip are generally used for objects that don't attach to the character.")]
		[Header("Item Settings")]
		public ItemTypes itemType = ItemTypes.NoEquip;
		[RequiredFieldAttribute("Unique name of this inventory item, matching it's 'Pickable' mate. This tells MultiGame what Inventory Item this is and allows it to keep track. It must match the 'Pickable' Inventory Key")]
		public string inventoryKey;
		[RequiredFieldAttribute("The pickable object associated with this item. MultiGame's local inventory system tracks objects through the Pickable")]
		public GameObject pickable;

		[Header("GUI Settings")]
		[Tooltip("Close inventory GUI automatically when selecting an item?")]
		public bool closeInventoryOnSelect = true;
		[RequiredFieldAttribute("Icon for this object in inventory",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public Texture2D icon;
		[RequiredFieldAttribute("Width of inventory icons")]
		public float iconSizeX = 32.0f;
		[RequiredFieldAttribute("Height of inventory icons")]
		public float iconSizeY = 32.0f;
	//	public float weight = 1.0f;

		public HelpInfo help = new HelpInfo("Inventory does not use object pooling directly. This component represents an inventory item that is currently in-use. Generally, this is something attached to the player like a " +
			"+5 Helmet of the Dunce or a machine gun. It must have a 'Pickable' prefab associated with it that has a 'Pickable' component on it and both must be inside a " +
			"'Resources' folder, otherwise you will get an error. Add an audio clip for stow/drop to the object and it will play automatically (Audio Source component).");

		[Tooltip("Log messages in the Console and log file when we interact with this component.")]
		public bool debug = false;

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

		
		[Header("Available Messages")]
		public MessageHelp stowHelp = new MessageHelp("Stow","Removes the item from the character heirarchy and puts it back in the Inventory");
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

		public MessageHelp dropHelp = new MessageHelp("Drop","Removes the item from the character heirarchy and instantiates it's pickable at this location");
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
}