using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Spawn")]
	public class UNetSpawn : NetworkBehaviour {

		[Tooltip("A list of prefabs we want to be able to spawn on the network. The prefabs are added to the spawn system once this object is spawned into the game.")]
		public GameObject[] prefabs;

		public GameObject spawnPoint;

		public enum AuthorityTypes {Serverside, Clientside};
		[Tooltip("Objects with 'Serverside' authority are controlled by the master client. Objects with 'Clientside' authority are client-side authoritative and act as the server " +
			"for that specific object.")]
		public AuthorityTypes authorityType = AuthorityTypes.Serverside;

		[RequiredFieldAttribute("Tag of the object that has authority on this client. This must be a Game Object in the scene.")]
		public string authoritativeObjectTag = "Player";

		public bool debug = false;

		/// <summary>
		/// A local reference to the object which has been spawned.
		/// </summary>
		[HideInInspector]
		public GameObject spawned;
		/// <summary>
		/// The authoritative object which has client authority on this machine.
		/// </summary>
		private GameObject authority;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("For spawning objects with client authority, requires a tag-designated object to be spawned in the game, otherwise " +
			"MultiGame will default to a server authority for the object." +
			"" +
			"To spawn an object, it must be added to the Prefabs list above. Then, call 'Spawn' and pass the exact name of the object you want to spawn. MultiGame will look through the list, " +
			"and spawn the first item with a matching name.");

		void Awake () {
			if (spawnPoint == null)
				spawnPoint = gameObject;
			foreach (GameObject _pFab in prefabs) {
				ClientScene.RegisterPrefab(_pFab);
			}
		}

		public MultiModule.MessageHelp spawnHelp = new MultiModule.MessageHelp("Spawn","Spawns an object over UNet, which must be added to the list of Prefabs above",4,"The name of the Prefab " +
			"we wish to spawn. Must match one of the entries from the list of Prefabs above.");
		public void Spawn (string _prefabName) {
			spawned = Instantiate(prefabs[GetSpawnableIndex(_prefabName)], spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
			if (GetSpawnableIndex(_prefabName) == -1) {
				Debug.LogError("U Net Spawn " + gameObject.name + " could not find " + _prefabName + " in it's list of Prefabs. Please make sure it's added to the Prefabs list and that you've " +
					"spelled the name correctly.");
				return;
			}

			if (authorityType == AuthorityTypes.Serverside || FindAuthority() == null) {
				if (debug)
					Debug.Log("U Net Spawn " + gameObject.name + " is spawning an object with serverside authority.");
				NetworkServer.Spawn(spawned);
			}
			else {
				if (debug)
					Debug.Log("U Net Spawn " + gameObject.name + " is spawning an object with clientside authority.");
				NetworkServer.SpawnWithClientAuthority(spawned, FindAuthority());
			}
		}

		public void SpawnAll () {
			foreach (GameObject _obj in prefabs) {
				spawned = Instantiate(_obj, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
				if (authorityType == AuthorityTypes.Serverside || FindAuthority() == null)
					NetworkServer.Spawn(spawned);
				else
					NetworkServer.SpawnWithClientAuthority(spawned, FindAuthority());
			}
		}

		/// <summary>
		/// Gets the index of the spawnable, returns -1 if not found
		/// </summary>
		/// <returns>The spawnable index.</returns>
		/// <param name="_prefabName">Prefab name.</param>
		public int GetSpawnableIndex (string _prefabName ) {
			int _ret = -1;

			for (int i = 0; i < prefabs.Length; i++) {
				if (prefabs[i].name == _prefabName)
					_ret = i;
			}

			return _ret;
		}

		private GameObject FindAuthority () {
			if (authority == null)
				authority = GameObject.FindGameObjectWithTag(authoritativeObjectTag);

			if (debug)
				Debug.Log("U Net Spawn " + gameObject.name + " authority is " + authority);

			return authority;
		}

	}
}