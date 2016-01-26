using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class ItemSpawner : MultiModule {

		[Tooltip("List of things we can spawn")]
		public GameObject[] items;
		[Tooltip("How many of each are available?")]
		public int[] itemCounts;
		[Tooltip("Should we spawn some as soon as we begin?")]
		public bool spawnOnStart = true;

		public HelpInfo help = new HelpInfo("This component spawns objects, but with a limited quantity.");

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

		void SpawnRandom () {
			int selector = Random.Range(0, items.Length);
			if (CheckItemAvailable(selector)) {
				SpawnItem(selector);
			}
			else {//itemCounts[selector] < 0
				itemCounts[selector] = 0;
			}

		}

		public bool CheckItemAvailable (int selector) {
			if (itemCounts[selector] > 0)
				return true;
			else
				return false;
		}

		void SpawnItem (float selector) {
			SpawnItem(Mathf.FloorToInt( selector));
		}

		void SpawnItem (int selector) {
			if (items.Length < selector)
				return;
			if (items[selector] != null) {
				Instantiate(items[selector], transform.position, transform.rotation);
			}
		}

	}
}