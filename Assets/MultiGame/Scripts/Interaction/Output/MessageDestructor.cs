using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class MessageDestructor : MultiModule {

		[Tooltip("Objects to spawn when we destroy this object")]
		public GameObject[] deathPrefabs;

		public HelpInfo help = new HelpInfo("This component allows things to be destroyed by receiving the 'Destruct' message. Very handy." +
			"\n\n" +
			"Drag and drop prefabs onto the 'Death Prefabs' list to make handy things come to life when this one dies.");

		[Tooltip("Sends a message to the console when we call 'Destruct'")]
		public bool debug = false;

		public void Destruct () {
			if (debug)
				Debug.Log("Destruct called on " + gameObject.name);
			foreach (GameObject deathPrefab in deathPrefabs)
				Instantiate(deathPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}

	}
}