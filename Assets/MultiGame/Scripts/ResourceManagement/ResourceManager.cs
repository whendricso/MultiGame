﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame
{
	[AddComponentMenu("MultiGame/Resource Management/Resource Manager")]
	[RequireComponent(typeof(Persistent))]
	public class ResourceManager : MultiModule
	{
		[Header("Important - Must be populated")]
		[Reorderable]
		public List<GameResource> startingResources = new List<GameResource>();

		[Header("IMGUI Settings")]
		[Tooltip("Should we show the resources using an immediate mode GUI? Not suitable for mobile.")]
		public bool showGui = true;
		[Tooltip("Normalized viewport rectangle indicating the screen area for the IMGUI. Numbers are a percentage of screen space between 0 and 1. Not suitable for mobile devices.")]
		public Rect guiArea = new Rect(.71f,.01f,.28f,.1f);
		[Tooltip("An optional skin for the IMGUI, if used")]
		public GUISkin guiSkin;

		private bool resourcesInitiated = false;

		/*[Tooltip("A list of resources in your game for a given player. Could be minerals, gold, mana, or even experience points. Anything that the player " +
			"spends, or needs to have a quantity and/or limit of in the game.")]*/
		public static List<GameResource> resources = new List<GameResource> ();

		//resource tick timers
		private List<float> currentTimers = new List<float> ();
		private string currentSelection = "";
		public HelpInfo help = new HelpInfo ("Resource Manager allows the player to have client-side resources like minerals, gold, or even experience points, " +
			"which when spent successfully will cause MultiGame to send messages. This can be used to unlock new items/abilities, purchase units, or anything really.");

		[System.Serializable]
		public class GameResource
		{
			[Tooltip("A unique name to identify this resource")]
			public string resourceName;
			[Tooltip("How much of this resource do we currently have?")]
			public float quantity;
			[Tooltip("The most we can have of this resource")]
			public float limit;
			[Tooltip("If non-zero, 'Tick Amount' resources will be added at this interval (in seconds)")]
			public float tickTime;
			[Tooltip("The amount of this resource we want to add each interval")]
			public float tickAmount;
			[Tooltip("If we're using IMGUI, should we show this resource in the list?")]
			public bool visible = true;
			[RequiredField("A UGUI Text component which we can use to display the current quantity and limit for this resource", RequiredFieldAttribute.RequirementLevels.Optional)]
			public Text text;

			public GameResource (float _quantity, float _limit, string _resourceName, float _tickTime, float _tickAmount)
			{
				visible = true;
				quantity = _quantity;
				limit = _limit;
				resourceName = _resourceName;
				tickTime = _tickTime;
				tickAmount = _tickAmount;
			}
		}

		void Start ()
		{
			if (!resourcesInitiated) {
				resourcesInitiated = true;
				foreach (GameResource _res in startingResources) {
					if (!resources.Contains(_res))
						resources.Add(_res);
				}
			}
			currentTimers.Clear ();
			for (int i = 0; i < resources.Count; i++) {
				currentTimers.Add (resources [i].tickTime);
			}
		}

		void OnGUI () {
			if (!showGui)
				return;
			GUI.skin = guiSkin;
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");
			GUILayout.BeginHorizontal();

			foreach(GameResource _res in resources) {
				if (_res.visible) {
					GUILayout.BeginVertical("", "box");
					GUILayout.Label(Mathf.Round(_res.quantity) + " / " + Mathf.Round(_res.limit));
					GUILayout.Label(_res.resourceName);
					GUILayout.EndVertical();
				}
			}

			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		void Update ()
		{
			for (int i = 0; i < resources.Count; i++) {
				if (resources[i].text != null)
					resources[i].text.text = (resources[i].quantity + " / " + resources[i].limit);

				if (resources[i].tickTime <= 0)
					break;
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
				resource.quantity = Mathf.Clamp(resource.quantity, 0f, resource.limit);
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
				resource.quantity = Mathf.Clamp(resource.quantity, 0f, resource.limit);
			}
		}

		public static void AddLimitByName(string _name, float _quantity) {
			foreach (GameResource resource in resources) {
				if (resource.resourceName == _name) {
					resource.limit += _quantity;
					if (resource.quantity > resource.limit) {
						resource.quantity = resource.limit;
					}
				}
				resource.quantity = Mathf.Clamp(resource.quantity, 0f, resource.limit);
			}
		}

		public static void AddQuantityByIndex (int _index, float _quantity)
		{
			resources [_index].quantity += _quantity;
			if (resources [_index].quantity > resources [_index].limit)
				resources [_index].quantity = resources [_index].limit;
			resources[_index].quantity = Mathf.Clamp(resources[_index].quantity, 0f, resources[_index].limit);
		}

		public static void DeductQuantityByIndex (int _index, float _quantity)
		{
			resources [_index].quantity -= _quantity;
			if (resources [_index].quantity < 0)
				resources [_index].quantity = 0;
			resources[_index].quantity = Mathf.Clamp(resources[_index].quantity, 0f, resources[_index].limit);
		}

		[Header("Available Messages")]
		public MessageHelp saveHelp = new MessageHelp("Save","Saves the current resources to Player Prefs. Works on all platforms.");
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

		public MessageHelp loadHelp = new MessageHelp("Load","Loads the resource list from Player Prefs. Works on all platforms.");
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

		public MessageHelp selectResourceHelp = new MessageHelp("SelectResource","Selects a resource by it's name from the list of Resources");
		public void SelectResource(string _resourceName) {
			currentSelection = _resourceName;
		}

		public MessageHelp adjustLimitHelp = new MessageHelp("AdjustLimit","How much do you want to adjust the limit for the currently named resource? Must be used after calling 'SelectResource' and passing the name of the resource you want to adjust the limit for.");
		public void AdjustLimit(float _adjustment) {
			AddLimitByName(currentSelection, _adjustment);
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Opens the IMGUI");
		public void OpenMenu () {
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Closes the IMGUI");
		public void CloseMenu () {
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toggles the IMGUI");
		public void ToggleMenu() {
			showGui = !showGui;
		}

		public MessageHelp showResourceHelp = new MessageHelp("ShowResource","Shows a given resource in the IMGUI (if applicable).",4,"The name of the resource we want to show.");
		public void ShowResource(string _name) {
			foreach (GameResource _res in resources) {
				if (_res.resourceName == _name)
					_res.visible = true;
			}
		}

		public MessageHelp hideResourceHelp = new MessageHelp("HideResource", "Hides a given resource in the IMGUI (if applicable).", 4, "The name of the resource we want to hide.");
		public void HideResource(string _name) {
			foreach (GameResource _res in resources) {
				if (_res.resourceName == _name)
					_res.visible = false;
			}
		}
	}
}