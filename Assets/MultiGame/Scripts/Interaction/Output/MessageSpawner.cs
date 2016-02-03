using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Message Spawner")]
	public class MessageSpawner : MultiModule {

		[Tooltip("Object we want to spawn")]
		public GameObject item;
		[Tooltip("Optional spawn point, spawns here if none")]
		public GameObject spawnPoint;
		[Tooltip("Should the spawned object inherit our velocity?")]
		public bool inheritVelocity = true;
		public bool debug = false;

		private GameObject spawnedEntity;

		public HelpInfo help = new HelpInfo("This component is a really easy way to spawn objects without a limit using the 'Spawn' message");

		void Start () {
			if (spawnPoint == null)
				spawnPoint = gameObject;
		}

		public void Spawn () {
			if(!enabled)
				return;
			if (debug)
				Debug.Log("Message Spawner " + gameObject.name + " spawned an " + item.name);
		
			spawnedEntity = Instantiate(item,spawnPoint.transform.position,spawnPoint.transform.rotation) as GameObject;
			Rigidbody _body = GetComponent<Rigidbody>();
			Rigidbody _newBody = spawnedEntity.GetComponent<Rigidbody>();
			if(inheritVelocity && (_body != null && _newBody != null))
				_newBody.AddForce( _body.velocity, ForceMode.VelocityChange);
		}

		public void SpawnAsChild() {
			Spawn();
			spawnedEntity.transform.SetParent(transform);
		}

	}
}