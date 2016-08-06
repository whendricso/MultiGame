using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame
{
	[AddComponentMenu("MultiGame/Resource Management/Resource Manager")]
	[RequireComponent(typeof(Persistent))]
	public class ResourceManager : MultiModule
	{
//		[Tooltip("Should we show the resources using a legacy GUI? Not suitable for mobile.")]
//		public bool showGui = false;

		[Tooltip("A list of resources in your game for a given player. Could be minerals, gold, mana, or even experience points. Anything that the player " +
			"spends, or needs to have a quantity and/or limit of in the game.")]
		public static List<GameResource>
			resources = new List<GameResource> ();

		//resource tick timers
		private List<float> currentTimers = new List<float> ();
		public HelpInfo help = new HelpInfo ("Resource Manager allows the player to have client-side resources like minerals, gold, or even experience points, " +
			"which when spent successfully will cause MultiGame to send messages. This can be used to unlock new items/abilities, purchase units, or anything really." +
			"\n----Messages:----\n" +
			"'Save' takes no parameter, and saves to PlayerPrefs\n" +
			"'Load' will load the resource from PlayerPrefs, if it exists");

		[System.Serializable]
		public class GameResource
		{
			public float quantity;
			public float limit;
			public string resourceName;
			public float tickTime;
			public float tickAmount;

			public GameResource (float _quantity, float _limit, string _resourceName, float _tickTime, float _tickAmount)
			{
				quantity = _quantity;
				limit = _limit;
				resourceName = _resourceName;
				tickTime = _tickTime;
				tickAmount = _tickAmount;
			}
		}

		void Start ()
		{
			currentTimers.Clear ();
			for (int i = 0; i < resources.Count; i++) {
				currentTimers.Add (resources [i].tickTime);
			}
		}

		void Update ()
		{
			for (int i = 0; i < resources.Count; i++) {
				currentTimers [i] -= Time.deltaTime;
				if (currentTimers [i] <= 0) {
					currentTimers [i] = resources [i].tickTime;
					AddQuantityByIndex (i, resources [i].tickAmount);
				}
			}
		}

		public static float GetQuantityByName (string _name)
		{
			float ret = 0;

			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name)
					ret = resource.quantity;
			}

			return ret;
		}

		public static void DeductQuantityByName (string _name, float _quantity)
		{
			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name)
					resource.quantity -= _quantity;
			}
		}

		public static void AddQuantityByName (string _name, float _quantity)
		{
			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name) {
					resource.quantity += _quantity;
					if (resource.quantity > resource.limit) {
						resource.quantity = resource.limit;
					}
				}
			}
		}

		public static void AddQuantityByIndex (int _index, float _quantity)
		{
			resources [_index].quantity += _quantity;
			if (resources [_index].quantity > resources [_index].limit)
				resources [_index].quantity = resources [_index].limit;
		}

		public static void ReduceQuantityByIndex (int _index, float _quantity)
		{
			resources [_index].quantity -= _quantity;
			if (resources [_index].quantity < 0)
				resources [_index].quantity = 0;
		}

		public void Save ()
		{
			if (!enabled)
				return;
			PlayerPrefs.SetInt ("numResources", resources.Count);
			for (int i = 0; i < resources.Count; i++) {
				PlayerPrefs.SetFloat ("resQuantity" + resources [i].resourceName, resources [i].quantity);
				PlayerPrefs.SetFloat ("resLimit" + resources [i].resourceName, resources [i].limit);
				PlayerPrefs.SetString ("resName" + resources [i].resourceName, resources [i].resourceName);
				PlayerPrefs.SetFloat ("resTickTime" + resources [i].resourceName, resources [i].tickTime);
				PlayerPrefs.SetFloat ("resTickAmount" + resources [i].resourceName, resources [i].tickAmount);
			}

			PlayerPrefs.Save ();
		}

		public void Load ()
		{
			if (!enabled)
				return;
			if (PlayerPrefs.HasKey ("numResources")) {
				resources.Clear ();
				for (int i = 0; i < PlayerPrefs.GetInt("numResources"); i++) {
					resources.Add(new GameResource(PlayerPrefs.GetFloat("resQuantity" + resources[i].resourceName), 
					                               PlayerPrefs.GetFloat("resLimit" + resources[i].resourceName), 
					                               PlayerPrefs.GetString("resName" + resources[i].resourceName),
					                               PlayerPrefs.GetFloat("resTickTime" + resources[i].resourceName),
					                               PlayerPrefs.GetFloat("resTickAmount" + resources[i].resourceName)));
				}
			}
		}

	}
}