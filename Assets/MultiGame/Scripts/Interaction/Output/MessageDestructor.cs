﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageDestructor")]
	public class MessageDestructor : MultiModule {

		public bool pool = false;
		[Reorderable]
		[Tooltip("Objects to spawn when we destroy this object. These aren't used in object pooling, so spawn with caution on mobile devices.")]
		public GameObject[] deathPrefabs;

		public HelpInfo help = new HelpInfo("This component allows things to be destroyed by receiving the 'Destruct' message. Very handy." +
			"\n\n" +
			"Drag and drop prefabs onto the 'Death Prefabs' list to make handy things come to life when this one dies.");

		[Tooltip("Sends a message to the console when we call 'Destruct'")]
		public bool debug = false;

		public MessageHelp destructHelp = new MessageHelp("Destruct","Deletes this object from the scene, and spawns the supplied list of 'Death Prefabs' if any");
		public void Destruct () {
			if (debug)
				Debug.Log("Destruct called on " + gameObject.name);
			foreach (GameObject deathPrefab in deathPrefabs)
				Instantiate(deathPrefab, transform.position, transform.rotation);

			if (!pool)
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}

	}
}