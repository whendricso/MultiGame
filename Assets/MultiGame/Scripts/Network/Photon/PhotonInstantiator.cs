using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {
	[AddComponentMenu("MultiGame/Network/Photon Instantiator")]
	[RequireComponent (typeof(PhotonView))]
	public class PhotonInstantiator : PhotonModule {

		[Tooltip("Where, relative to this object's position, should we spawn the object?")]
		public Vector3 spawnOffset = Vector3.zero;
		[Tooltip("Should this be spawned as a scene object, and be owned by the Master Client? (Otherwise, this client is authoritative over it)")]
		public bool asSceneObject = false;
		public MessageManager.ManagedMessage messageToSpawnedEntity;
		[Tooltip("Should the object we spawned have a relative velocity to ours?")]
		public bool inheritVelocity = true;
		[Tooltip("If true, we will attempt to determine the team of this object, and send it to the spawned object by using 'SetTeam' with an integer representing that team's index.")]
		public bool inheritTeam = true;

		private GameObject spawnedEntity;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Instantiator takes the 'Spawn' message, with a string parameter representing the name of the prefab you want to " +
			"spawn. The prefab must be in a 'Resources' folder, or Photon will throw an error and the object will fail instantiation. Message To Spawned Entity is sent to the prefab after it " +
			"spawns.");

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref messageToSpawnedEntity, gameObject);
		}

		public void Spawn (string prefabResourceName) {
			if (PhotonNetwork.room == null)
				return;
			if (!asSceneObject)
				spawnedEntity = PhotonNetwork.Instantiate(prefabResourceName, transform.position + spawnOffset, transform.rotation, 0);
			else
				spawnedEntity = PhotonNetwork.InstantiateSceneObject(prefabResourceName, transform.position + spawnOffset, transform.rotation, 0, null);

			spawnedEntity.SendMessage("SetOwner", transform.root.gameObject, SendMessageOptions.DontRequireReceiver);//set the owner in case we're spawning a 'Bullet' projectile

			if (inheritVelocity) {
				Rigidbody _myRigid = GetComponent<Rigidbody>() as Rigidbody;
				Rigidbody _otherRigid = spawnedEntity.GetComponent<Rigidbody>() as Rigidbody;

				if (_myRigid != null && _otherRigid != null) {
					_otherRigid.AddForce(_myRigid.velocity, ForceMode.VelocityChange);
				}
			}

			if (inheritTeam) {
				PhotonTeamObject _tobj = transform.root.GetComponentInChildren<PhotonTeamObject>();
				if (_tobj != null)
					MessageManager.Send(new MessageManager.ManagedMessage(
						spawnedEntity, 
						"SetTeam", 
						MessageManager.ManagedMessage.SendMessageTypes.Broadcast, 
						_tobj.currentTeam.ToString(), 
						MessageManager.ManagedMessage.ParameterModeTypes.Integer
					));
			}

			if (!string.IsNullOrEmpty( messageToSpawnedEntity.message))
				MessageManager.SendTo(messageToSpawnedEntity, spawnedEntity);
		}

		public void SpawnAsChild(string prefabResourceName) {
			Spawn(prefabResourceName);
			spawnedEntity.transform.SetParent(transform);
		}
		
	}
}