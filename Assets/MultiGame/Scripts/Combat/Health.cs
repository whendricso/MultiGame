using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Health")]
	public class Health : MultiModule {

		[Header("Basic Settings")]
		[RequiredFieldAttribute("How much health do we start with?")]
		public float hp = 100.0f;
		[RequiredFieldAttribute("How much health can we have?")]
		public float maxHP = 100.0f;
		[Header("Destruction Settings")]
		[Tooltip("Do we destroy the object when health runs out?")]
		public bool autodestruct = true;
		[Tooltip("What should we spawn when we die from HP loss?")]
		public GameObject[] deathPrefabs;
		//public GameObject deathCam;//optional camera to be spawned, which watches the first death prefab

		[Header("GUI Settings")]
		[Tooltip("If using the Unity UI, create a scroll bar for the health and drop a reference to it here. The handle of the scrollbar is resized to show the health amount. This can be used to create " +
			"either a health bar near the object in the scene, or displayed as a HUD. See the Unity GUI documentation/tutorials for more information.")]
		public Scrollbar uIhealthBar;

		[Header("Immediate Mode (Legacy) GUI settings")]
		[RequiredFieldAttribute("What skin should we use for the Legacy GUI",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GUISkin guiSkin;
		[Tooltip("Should we show a legacy Unity GUI? NOTE: Not suitable for mobile devices.")]
		public bool showHealthBarGUI = false;
		[Tooltip("Normalized viewport rectangle describing the area of the health bar, values between 0 and 1")]
		public Rect healthBar = new Rect(.2f, .01f, .4f, .085f);
		[Tooltip("Should we hide the health bar when health is full?")]
		public bool autoHide = false;//if health is full, hide this bar
		[Tooltip("Should we color this bar to differentiate it from others?")]
		public Color barColor = Color.white;
		[Header("Message Senders")]
		[Tooltip("When we run out of health, what message should we send?")]
		public MessageManager.ManagedMessage healthGoneMessage;
		[Tooltip("When we are hit, what message should we send?")]
		public MessageManager.ManagedMessage hitMessage;

		public HelpInfo help = new HelpInfo("This flexible Health implementation is fully compatible with all MultiGame systems and forms the basis for combat and death in your " +
			"game worlds. It receives the 'ModifyHealth' message with a floating point value. Positive numbers heal it, negative numbers damage it. Make anything killable! Fill out message senders above to " +
			"send messages to other MultiGame components.");

		public bool debug = false;

		
		void Start () {
	//		hp = maxHP;
			if (healthGoneMessage.target == null)
				healthGoneMessage.target = gameObject;
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref healthGoneMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref hitMessage, gameObject);
		}
		
		void Update () {
	//		if (hp <= 0)
	//			Die();
			if (hp > maxHP)
				hp = maxHP;

			if (uIhealthBar != null)
				uIhealthBar.size = hp / maxHP;
		}
		
		void OnGUI () {
			if (hp == maxHP && autoHide)
				return;
			if (showHealthBarGUI) {
				GUI.skin = guiSkin;
				GUI.color = barColor;
				GUILayout.BeginArea(new Rect(healthBar.x * Screen.width, healthBar.y * Screen.height, (healthBar.width * Screen.width) * (hp / maxHP), healthBar.height * Screen.height));
				GUILayout.Box("Health");
				GUILayout.EndArea();
			}
		}

		[Header("Available Messages")]
		public MessageHelp dieHelp = new MessageHelp("Die","Kill this object immediately!");
		public void Die() {
			if (debug)
				Debug.Log("Health component " + gameObject.name + " has died!");
			MessageManager.Send( healthGoneMessage);

			
			if (deathPrefabs.Length > 0)
			for (int i = 0; i < deathPrefabs.Length; i++) {
				/*GameObject dFab = */Instantiate(deathPrefabs[i], transform.position, transform.rotation)/* as GameObject*/;
			}
			if (!autodestruct)
				return;
			Destroy(gameObject);
		}

		public MessageHelp modifyHealthHelp = new MessageHelp("ModifyHealth","Change the health this object has", 3, "Amount to change the health by. Positive to increase, negative to decrease.");
		public void ModifyHealth (float val) {
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			MessageManager.Send(hitMessage);
			hp += val;
			if (hp <= 0.0f) {
				Die ();
				if (!autodestruct)
					return;
			}
		}

		public MessageHelp modifyMaxHealthHelp = new MessageHelp("ModifyMaxHealth","Change the maximum health this object can have", 3, "Amount to change the max health by. Positive to increase, negative to decrease.");
		public void ModifyMaxHealth (float val) {
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			maxHP += val;
		}
	}
}
//Copyright 2014 William Hendrickson
