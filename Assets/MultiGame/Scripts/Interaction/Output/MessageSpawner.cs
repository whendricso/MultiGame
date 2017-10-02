using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Message Spawner")]
	public class MessageSpawner : MultiModule {

		[RequiredFieldAttribute("Object we want to spawn", RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject item;
		[RequiredFieldAttribute("Optional spawn point, spawns here if none", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject spawnPoint;
		[Tooltip("Should the spawned object inherit our velocity?")]
		public bool inheritVelocity = true;
		public bool debug = false;

		private GameObject spawnedEntity;

		public HelpInfo help = new HelpInfo("Simply send 'Spawn' or 'SpawnAsChild' to this component to instantiate 'Item' at 'SpawnPoint'. 'Item' does not have to be an Inventory Pickable, but it can be if you wish. It can be " +
			"any object that you want to instantiate into your game for any reason. For example you could spawn anthropomorphic bullets out of a cartoon cannon in your level using this, or you could spawn items or even " +
			"invisible sound effect objects. Really, any prefab is a valid choice.");

		void Start () {
			if (spawnPoint == null)
				spawnPoint = gameObject;
		}

		public MessageHelp spawnHelp = new MessageHelp("Spawn","Spawns the 'Item' at 'Spawn Point'.");
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

		public MessageHelp spawnAsChildHelp = new MessageHelp("SpawnAsChild","Spawns the 'Item' at 'Spawn Point' and then parents it to the spawn point.");
		public void SpawnAsChild() {
			Spawn();
			spawnedEntity.transform.SetParent(spawnPoint.transform);
		}

	}
}