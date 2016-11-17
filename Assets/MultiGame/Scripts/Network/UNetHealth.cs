using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Health")]
//	[RequireComponent(typeof(NetworkIdentity))]
	public class UNetHealth : NetworkBehaviour {
		
		[RequiredFieldAttribute("How much health do we start with?")][SyncVar]
		public float hp = 100.0f;
		[RequiredFieldAttribute("How much health can we have?")][SyncVar]
		public float maxHP = 100.0f;
		[Tooltip("Do we destroy the object when health runs out?")]
		public bool autodestruct = true;
		[Tooltip("What should we spawn when we die from HP loss?")]
		public GameObject[] deathPrefabs;
		//public GameObject deathCam;//optional camera to be spawned, which watches the first death prefab

		[RequiredFieldAttribute("What skin should we use for the Legacy GUI",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GUISkin guiSkin;
		[Tooltip("Should we show a legacy Unity GUI? NOTE: Not suitable for mobile devices.")]
		public bool showHealthBarGUI = false;
		[Tooltip("Normalized viewport rectangle describing the area of the health bar, values between 0 and 1")]
		public Rect healthBar = new Rect(.2f, .01f, .4f, .085f);
		[Tooltip("Should we hide the health bar when health is full?")]
		public bool autoHide = true;//if health is full, hide this bar
		[Tooltip("Should we color this bar to differentiate it from others?")]
		public Color barColor = Color.white;
		[Tooltip("When we run out of health, what message should we send?")]
		public MessageManager.ManagedMessage healthGoneMessage;
		[Tooltip("When we are hit, what message should we send?")]
		public MessageManager.ManagedMessage hitMessage;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("This generic Health implementation is fully compatible with all MultiGame systems and forms the basis for combat and death in your " +
			"game worlds. It receives the 'ModifyHealth' message with a floating point value. Make anything killable!");

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
			if (!isLocalPlayer)
				return;
			if (hp > maxHP)
				hp = maxHP;
		}
		
		void OnGUI () {
			if (!isLocalPlayer)
				return;
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

		public MultiModule.MessageHelp dieHelp = new MultiModule.MessageHelp("Die","Kill this object immediately!");
		public void Die () {
			if (!hasAuthority)
				return;
			CmdDie();
		}

		[Command]
		public void CmdDie() {
			if (debug)
				Debug.Log("Health component " + gameObject.name + " has died!");
			MessageManager.Send( healthGoneMessage);

			
			if (deathPrefabs.Length > 0)
			for (int i = 0; i < deathPrefabs.Length; i++) {
				/*GameObject dFab = */Instantiate(deathPrefabs[i], transform.position, transform.rotation)/* as GameObject*/;
			}
			if (!autodestruct)
				return;
			NetworkServer.Destroy(gameObject);//Destroy(gameObject);
		}

		public MultiModule.MessageHelp modifyHealthHelp = new MultiModule.MessageHelp("ModifyHealth","Change the health this object has", 3, "Amount to change the health by. Positive to increase, negative to decrease.");
		public void ModifyHealth (float val) {
			if (!hasAuthority)
				return;
			CmdUpdateHealth(val);
		}

		[Command]
		private void CmdUpdateHealth (float val) {
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			MessageManager.Send(hitMessage);
			hp += val;
			if (hp <= 0.0f) {
				CmdDie ();
			}
		}

		public MultiModule.MessageHelp modifyMaxHealthHelp = new MultiModule.MessageHelp("ModifyMaxHealth","Change the maximum health this object can have", 3, "Amount to change the max health by. Positive to increase, negative to decrease.");
		public void ModifyMaxHealth (float val) {
			if (!hasAuthority)
				return;
			CmdUpdateMaxHealth(val);
		}

		[Command]
		private void CmdUpdateMaxHealth (float val) {
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			maxHP += val;
		}
	}
}
//Copyright 2014 William Hendrickson
