﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	/*
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(PhotonInstantiator))]
	*/
	public class PhotonCharacterLoadoutSpawner : Photon.MonoBehaviour {
		/*
		public List<Loadout> possibleLoadouts = new List<Loadout>();

		public MultiMenu.HelpInfo help = new MultiModule.HelpInfo("");

		[System.Serializable]
		public class Loadout {
			public string name;
			public string unitPrefab;

			public List<PhotonCharacterLoadoutSpawner.KitItem> kitItems = new List<PhotonCharacterLoadoutSpawner.KitItem>();

			//constructor
			public Loadout(List<PhotonCharacterLoadoutSpawner.KitItem> _items) {
				kitItems = _items;
			}

		}

		public class KitItem {
			public string prefabName;
			public int cost = 1;
			public GameObject slot;

			public KitItem (string _prefabName) {
				prefabName = _prefabName;
			}

			public KitItem (string _prefabName, int _cost) {
				prefabName = _prefabName;
				cost = _cost;
			}

			public KitItem (string _prefabName, int _cost, int _slot) {
				prefabName = _prefabName;
				cost = _cost;
				slot = _slot;
			}
		}

		public void SpawnSelected (int _selector) {
			if (_selector < this.possibleLoadouts.Count)
				photonView.RPC("RemoteSpawnLoadout", PhotonTargets.AllBuffered, _selector);


		}
	*/
	}
}