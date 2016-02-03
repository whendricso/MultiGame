﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Collectible")]
	public class Collectible : MultiModule {

		[Tooltip("Optional reference to the collection manager, which must exist in the scene for Collectible to work")]
		public CollectionManager collectionManager;
	//	public bool mustBePlayer = true;
		[Tooltip("Object to spawn when we are collected")]
		public GameObject deathPrefab;

		public HelpInfo help = new HelpInfo("This component implements a collectible object. This requires that a CollectionManager be present somewhere in the scene (only one" +
			" manager should be present, each collectible needs one of these components however)");

		void Start () {
			if (collectionManager == null)
				collectionManager = FindObjectOfType<CollectionManager>();
			if (collectionManager == null) {
				Debug.LogError("Collectible " + gameObject.name + " needs a CollectionManager in the scene to function!");
				enabled = false;
				return;
			}
		}
		
		void OnTriggerEnter (Collider other) {
			if (other.gameObject.tag != "Player")
				return;
			collectionManager.gameObject.SendMessage("Collect", SendMessageOptions.DontRequireReceiver);
			if (deathPrefab != null)
				Instantiate(deathPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}

		void OnCollisionEnter (Collision collision) {
			if (collision.collider.gameObject.tag != "Player")
				return;
			collectionManager.gameObject.SendMessage("Collect", SendMessageOptions.DontRequireReceiver);
			if (deathPrefab != null)
				Instantiate(deathPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}