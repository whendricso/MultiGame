using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Message Spawner")]
	public class MessageSpawner : MultiModule {

		[Header("Pooling Options")]
		[Tooltip("Should objects be added to a list and respawned from the pool? Objects are available to pool if they are disabled in the Heirarchy when a 'Spawn' event occurs.")]
		public bool poolObjects = false;

		[Header("Spawn Options")]
		[RequiredField("Object we want to spawn", RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject item;
		[RequiredField("Optional spawn point, spawns here if none", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject spawnPoint;
		[Tooltip("Should the spawned object inherit our velocity?")]
		public bool inheritVelocity = true;
		public bool debug = false;

		private GameObject spawnedEntity;
		GameObject spawnable;//if we're pooling objects, spawn this if it's found in the pool

		private List<GameObject> objectPool = new List<GameObject>();

		public HelpInfo help = new HelpInfo("Simply send 'Spawn' or 'SpawnAsChild' to this component to instantiate 'Item' at 'SpawnPoint'. 'Item' does not have to be an Inventory Pickable, but it can be if you wish. It can be " +
			"any object that you want to instantiate into your game for any reason. For example you could spawn anthropomorphic bullets out of a cartoon cannon in your level using this, or you could spawn items or even " +
			"invisible sound effect objects. Really, any prefab is a valid choice.");

		void Awake () {
			if (spawnPoint == null)
				spawnPoint = gameObject;
		}

		public MessageHelp spawnHelp = new MessageHelp("Spawn","Spawns the 'Item' at 'Spawn Point'.");
		public void Spawn () {
			if(!enabled)
				return;
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Message Spawner " + gameObject.name + " spawned an " + item.name);

			if (!poolObjects)
				spawnedEntity = Instantiate(item, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
			else {
				spawnable = FindPooledObject();
				if (spawnable == null) {
					spawnedEntity = Instantiate(item, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
					if (spawnedEntity.GetComponent<CloneFlagRemover>() == null)
						spawnedEntity.AddComponent<CloneFlagRemover>();
					objectPool.Add(spawnedEntity);
				}
				else {
					SpawnFromPool(spawnable);
					spawnedEntity = spawnable;
				}
			}
			Rigidbody _body = GetComponent<Rigidbody>();
			Rigidbody _newBody = spawnedEntity.GetComponent<Rigidbody>();
			if(inheritVelocity && (_body != null && _newBody != null))
				_newBody.AddForce( _body.velocity, ForceMode.VelocityChange);
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

		//private Rigidbody spawnedBody;
		private void SpawnFromPool(GameObject obj) {
			obj.transform.position = spawnPoint.transform.position;
			obj.transform.rotation = spawnPoint.transform.rotation;
			obj.SetActive(true);
			obj.BroadcastMessage("ReturnFromPool",SendMessageOptions.DontRequireReceiver);
			//spawnedBody = obj.GetComponent<Rigidbody>();
		}

		public MessageHelp spawnAsChildHelp = new MessageHelp("SpawnAsChild","Spawns the 'Item' at 'Spawn Point' and then parents it to the spawn point.");
		public void SpawnAsChild() {
			Spawn();
			spawnedEntity.transform.SetParent(spawnPoint.transform);
		}

	}
}