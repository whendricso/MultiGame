using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	/// <summary>
	/// Minion spawn will raycast downwards on Start, and try to spawn units
	/// </summary>
	[AddComponentMenu("MultiGame/Inventory/Minion Spawn")]
	public class MinionSpawn : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("What objects should we look for to spawn on? This performs a raycast straight down to find out if we're deploying onto a valid surface. Generally, this will occur at the Instantiation Transform designated on the Inventory component.")]
		public LayerMask rayMask;

		[Header("Spawn Settings")]
		[RequiredFieldAttribute("How far down from the origin are we willing to look for a surface to spawn on?")]
		public float rayDistance = 2.0f;
		[RequiredFieldAttribute("Unit we are attempting to spawn")]
		public GameObject minion;
		[RequiredFieldAttribute("A reference to an inventory item that will be added back in to the inventory in case we fail to spawn anything")]
		public GameObject activeObject;

		[HideInInspector]
		public ActiveObject activ;

		public HelpInfo help = new HelpInfo("This component handles the unique case when you are able to spawn minions from inventory items. It will return the item to inventory" +
			" if spawning fails. To use it, attach it to an empty object that is instantiated at 'Instantiation Transform' when used from inventory. If the criteria for spawning " +
			"are met, it will destroy itself and spawn a minion, otherwise it will return the item to your inventory.");
		
		void Start () {
			activ = GetComponent<ActiveObject>();
			if (activ == null) {
				Debug.LogError("Minion Spawn " + gameObject.name + " needs an Active Object component added to it.");
				enabled = false;
				return;
			}
			if (activeObject == null) {
				Debug.LogError("Minion Spawn " + gameObject.name + " needs an Active Object game object assigned in the inspector, in case of spawn failure.");
				enabled = false;
				return;
			}
			//GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			RaycastHit hinfo;
			bool didHit = Physics.Raycast(transform.position, Vector3.down, out hinfo, rayDistance, rayMask );
			if (!didHit) {
				ReturnToInventory();
				return;
			}
			Instantiate(minion, hinfo.point, transform.rotation);
			Destroy(gameObject);
		}
		
		void ReturnToInventory() {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			Inventory inventory = player.GetComponent<Inventory>();
			if (player == null)
				return;
			if (inventory == null)
				return;
			KeyValuePair<string, GameObject> kvp = new KeyValuePair<string, GameObject>(activ.inventoryKey, activeObject);
			inventory.Pick(kvp);
			Destroy(gameObject);
		}
	}
}