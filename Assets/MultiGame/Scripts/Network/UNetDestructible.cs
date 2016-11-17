using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Destructible")]
	public class UNetDestructible : NetworkBehaviour {

		[Tooltip("Objects to spawn over the network when destroyed.")]
		public GameObject[] deathPrefabs;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("This component allows things to be destroyed by receiving the 'Destruct' message. Very handy." +
			"\n\n" +
			"Drag and drop prefabs onto the 'Death Prefabs' list to make handy things come to life when this one dies.");

		[Tooltip("Sends a message to the console when we call 'Destruct'")]
		public bool debug = false;

		public MultiModule.MessageHelp destructHelp = new MultiModule.MessageHelp("Destruct","Deletes this object from the scene, and spawns the supplied list of 'Death Prefabs' if any");
		public void Destruct () {
			if (!hasAuthority)
				return;
			CmdNetDestruct();
		}

		[Command]
		public void CmdNetDestruct () {
			
			if (debug)
				Debug.Log("Destruct called on " + gameObject.name);
			foreach (GameObject deathPrefab in deathPrefabs)
				Instantiate(deathPrefab, transform.position, transform.rotation);

			NetworkServer.Destroy(gameObject);
		}

	}
}