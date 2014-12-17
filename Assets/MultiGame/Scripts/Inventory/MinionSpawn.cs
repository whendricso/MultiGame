using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Minion spawn will raycast downwards on Start, and try to spawn units, otherwise
/// a failure prefab will be spawned.
/// Minions follow the player's command.
/// </summary>
public class MinionSpawn : MonoBehaviour {
	
	public GameObject minion;
	public float rayDistance = 2.0f;
	public LayerMask rayMask;
	public GameObject activeObject;
	[HideInInspector]
	public ActiveObject activ;
	
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