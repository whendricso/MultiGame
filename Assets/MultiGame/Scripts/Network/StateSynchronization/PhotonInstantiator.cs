using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(PhotonView))]
public class PhotonInstantiator : Photon.MonoBehaviour {

	public Vector3 spawnOffset = Vector3.zero;
	public bool asSceneObject = false;
	public MessageManager.ManagedMessage messageToSpawnedEntity;

	private GameObject spawnedEntity;

	public void Spawn (string prefabResourceName) {
		if (PhotonNetwork.room == null)
			return;
		if (!asSceneObject)
			spawnedEntity = PhotonNetwork.Instantiate(prefabResourceName, transform.position + spawnOffset, transform.rotation, 0);
		else
			spawnedEntity = PhotonNetwork.InstantiateSceneObject(prefabResourceName, transform.position + spawnOffset, transform.rotation, 0, null);

		if (!string.IsNullOrEmpty( messageToSpawnedEntity.message))
			MessageManager.SendTo(messageToSpawnedEntity, spawnedEntity);
	}
	
}