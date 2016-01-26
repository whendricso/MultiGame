using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

public class ResourceManager : MultiModule {

		public static List<GameResource> resources = new List<GameResource>();

		private List<float> currentTimers = new List<float>();

		[System.Serializable]
		public class GameResource {
			public float quantity;
			public float limit;
			public string resourceName;
			public float tickTime;
			public float tickAmount;

			public GameResource (float _quantity, float _limit, string _resourceName, string _tickTime, string _tickAmount) {
				quantity = _quantity;
				limit = _limit;
				resourceName = _resourceName;
			}
		}

		void Start () {
			currentTimers.Clear();
			for (int i = 0; i < resources.Count; i++) {
				currentTimers.Add(resources[i].tickTime);
			}
		}

		void Update () {
			for (int i = 0; i < resources.Count; i++) {
				currentTimers[i] -= Time.deltaTime;
				if (currentTimers[i] <= 0) {
					currentTimers[i] = resources[i].tickTime;
					AddQuantityByIndex(i, resources[i].tickAmount);
				}
			}
		}

		public static float GetQuantityByName (string _name) {
			float ret = 0;

			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name)
					ret = resource.quantity;
			}

			return ret;
		}

		public static void DeductQuantityByName(string _name, float _quantity) {
			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name)
					resource.quantity -= _quantity;
			}
		}

		public static void AddQuantityByName(string _name, float _quantity) {
			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name) {
					resource.quantity += _quantity;
					if (resource.quantity > resource.limit) {
						resource.quantity = resource.limit;
					}
				}
			}
		}

		public static void AddQuantityByIndex(int _index, float _quantity) {
			resources[_index].quantity += _quantity;
			if (resources[_index].quantity > resources[_index].limit)
				resources[_index].quantity = resources[_index].limit;
		}

		public static void ReduceQuantityByIndex(int _index, float _quantity) {
			resources[_index].quantity -= _quantity;
			if(resources[_index].quantity < 0)
				resources[_index].quantity = 0;
		}

	}
}