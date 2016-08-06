using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Item Spawner")]
	public class ItemSpawner : MultiModule {

		[Tooltip("List of things we can spawn")]
		public GameObject[] items;
		[Tooltip("How many of each are available?")]
		public int[] itemCounts;
		[Tooltip("Should we spawn some as soon as we begin?")]
		public bool spawnOnStart = true;

		public HelpInfo help = new HelpInfo("This component spawns objects, but with a limited quantity. To use, supply a list of items, for each item supplying a corresponding count. so, item #3 would have 6 available" +
			"if itemCounts #3 == 6");

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
					Instantiate(items[selector], transform.position, transform.rotation);
					itemCounts[selector]--;
				}
			}
		}

	}
}