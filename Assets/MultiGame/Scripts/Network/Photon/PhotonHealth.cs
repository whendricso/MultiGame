using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Health")]
	public class PhotonHealth : PhotonModule {

		[RequiredFieldAttribute("How much health do we start with?")]
		public float hp = 100.0f;
		[RequiredFieldAttribute("How much health can we have?")]
		public float maxHP = 100.0f;
		[Tooltip("Do we destroy the object when health runs out?")]
		public bool autodestruct = true;
		[Tooltip("What should we spawn when we die from HP loss?")]
		public GameObject[] deathPrefabs;
		//public GameObject deathCam;//optional camera to be spawned, which watches the first death prefab

		[Tooltip("If using the Unity UI, create a scroll bar for the health and drop a reference to it here. The handle of the scrollbar is resized to show the health amount.")]
		public Scrollbar uIhealthBar;

		[Tooltip("Should we show a legacy Unity GUI? NOTE: Not suitable for mobile devices.")]
		public bool showHealthBarGUI = false;
		[RequiredFieldAttribute("What skin should we use for the Legacy GUI",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GUISkin guiSkin;
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

		private PhotonView view;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("This generic Health implementation is fully compatible with all MultiGame systems and forms the basis for combat and death in your " +
			"game worlds. It receives the 'ModifyHealth' message with a floating point value. Make anything killable!");

		public bool debug = false;


		void Start () {
			//		hp = maxHP;
			view = GetView();
			if (view == null) {
				Debug.LogError ("Photon Health " + gameObject.name + " needs to be attached to an object with a Photon View");
				enabled = false;
				return;
			}
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

		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.isWriting) {
				stream.SendNext (hp);
				stream.SendNext (maxHP);
			} else {
				hp = (float)stream.ReceiveNext ();
				maxHP = (float)stream.ReceiveNext ();
			}
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

		public MultiModule.MessageHelp dieHelp = new MultiModule.MessageHelp("Die","Kill this object immediately!");
		public void Die() {
			if (!view.isMine)
				return;
			if (debug)
				Debug.Log("Health component " + gameObject.name + " has died!");
			view.RPC ("DieRPC", PhotonTargets.AllBuffered);
		}

		[PunRPC]
		private void DieRPC () {
			MessageManager.Send( healthGoneMessage);

			if (deathPrefabs.Length > 0)
				for (int i = 0; i < deathPrefabs.Length; i++) {
					/*GameObject dFab = */Instantiate(deathPrefabs[i], transform.position, transform.rotation)/* as GameObject*/;
				}
			if (!autodestruct)
				return;
			Destroy(gameObject);
		}

		public MultiModule.MessageHelp modifyHealthHelp = new MultiModule.MessageHelp("ModifyHealth","Change the health this object has", 3, "Amount to change the health by. Positive to increase, negative to decrease.");
		public void ModifyHealth (float val) {
			if (!view.isMine)
				return;
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			view.RPC ("ModifyHealthRPC", PhotonTargets.AllBuffered,val);
		}

		[PunRPC]
		private void ModifyHealthRPC (float val) {
			MessageManager.Send(hitMessage);
			hp += val;
			if (hp <= 0.0f) {
				Die ();
				if (!autodestruct)
					return;
			}
		}

		public MultiModule.MessageHelp modifyMaxHealthHelp = new MultiModule.MessageHelp("ModifyMaxHealth","Change the maximum health this object can have", 3, "Amount to change the max health by. Positive to increase, negative to decrease.");
		public void ModifyMaxHealth (float val) {
			if (!view.isMine)
				return;
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			view.RPC ("ModifyMaxHealthRPC",PhotonTargets.AllBuffered, val);
		}

		[PunRPC]
		private void ModifyMaxHealthRPC (float val) {
			maxHP += val;

		}
	}
}