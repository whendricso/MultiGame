using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

//	[AddComponentMenu("MultiGame/General/Item Spawner")]
	public class ItemSpawner : MultiModule {

		[Tooltip("Should objects be added to a list and respawned from the pool? Objects are available to pool if they are disabled in the Heirarchy when a 'SpawnItem' event occurs.")]
		public bool poolObjects = false;
		[Reorderable]
		[Tooltip("List of things we can spawn")]
		public GameObject[] items;
		[Reorderable]
		[Tooltip("How many of each are available?")]
		public int[] itemCounts;
		[Tooltip("Should we spawn some as soon as we begin?")]
		public bool spawnOnStart = true;

		private GameObject spawnedEntity;
		private GameObject spawnable;
		private List<GameObject> objectPool = new List<GameObject>();


		public HelpInfo help = new HelpInfo("This component spawns objects, but with a limited quantity. To use, supply a list of items, for each item supplying a corresponding count. So, item #3 would have 6 available" +
			"if itemCounts #3 == 6. Each list must have exactly the same size");

		// Use this for initialization
		void Start () {
			if (items.Length != itemCounts.Length) {
				Debug.LogError("Item Spawner" + gameObject.name + " needs matching items and item counts in the inspector.");
				enabled = false;
				return;
			}

			if (spawnOnStart) {
				SpawnRandom();
			}
		}

		public MessageHelp spawnRandomHelp = new MessageHelp("SpawnRandom","Spawns a random item from the list of 'Items'.");
		public void SpawnRandom () {
			int selector = Random.Range(0, items.Length);
			if (CheckItemAvailable(selector)) {
				SpawnItem(selector);
			}
			else {//itemCounts[selector] < 0
				itemCounts[selector] = 0;
			}

		}
			
		bool CheckItemAvailable (int selector) {
			if (itemCounts[selector] > 0)
				return true;
			else
				return false;
		}
			
		public void SpawnItem (float selector) {
			SpawnItem(Mathf.FloorToInt( selector));
		}

		public MessageHelp spawnItemHelp = new MessageHelp("SpawnItem","Spawns an item from the list of 'Items' ", 2, "The index of the item in the 'Items' list you want to spawn. An item must be available.");
		public void SpawnItem (int selector) {
			if (items.Length < selector)
				return;
			if (items[selector] != null) {
				if (CheckItemAvailable(selector)) {
					if (!poolObjects)
						Instantiate(items[selector], transform.position, transform.rotation);
					else {
						spawnable = FindPooledObject();
						if (spawnable == null) {
							spawnedEntity = Instantiate(items[selector], transform.position, transform.rotation) as GameObject;
							if (spawnedEntity.GetComponent<CloneFlagRemover>() == null)
								spawnedEntity.AddComponent<CloneFlagRemover>();
							objectPool.Add(spawnedEntity);
						}
						else {
							SpawnFromPool(spawnable);
							spawnedEntity = spawnable;
						}
					}
					itemCounts[selector]--;
				}
			}
		}

		/// <summary>
		/// Searches the heirarchy for a pooled (disabled) object
		/// </summary>
		/// <returns></returns>
		private GameObject FindPooledObject() {
			GameObject ret = null;

			foreach (GameObject obj in objectPool) {
				if (!obj.activeInHierarchy) {
					ret = obj;
					break;
				}
			}

			return ret;
		}

		private void SpawnFromPool(GameObject obj) {
			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			obj.SetActive(true);
			obj.BroadcastMessage("ReturnFromPool", SendMessageOptions.DontRequireReceiver);
		}

	}
}