using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Health")]
	public class Health : MultiModule {

		[Tooltip("Should this object be disabled instead of destroyed so that it can be pooled for later use?")]
		public bool pool = false;

		[Header("Basic Settings")]
		public bool invulnerable = false;
		[RequiredFieldAttribute("How much health do we start with?")]
		public float hp = 100.0f;
		[RequiredFieldAttribute("How much health can we have?")]
		public float maxHP = 100.0f;
		[Header("Destruction Settings")]
		[Tooltip("Do we destroy the object when health runs out?")]
		public bool autodestruct = true;
		[RequiredFieldAttribute("Specify a key to save the health in Player Prefs. Will load when any instance of this object is instantiated. If you don't want to save, just leave this blank.",RequiredFieldAttribute.RequirementLevels.Optional)]
		public string autoSaveKey = "";

		[Header("Game Feel")]
		[Tooltip("A prefab we spawn when we get hit. Perhaps a particle effect?")]
		public GameObject hitPrefab;
		[Tooltip("A spawn point where you want the hit prefab to appear (perhaps near the center?)")]
		public GameObject hitPrefabSpawnPoint;
		public AudioSource hitAudioSource;
		[Tooltip("A sound you wish to play when we get hit")]
		public AudioClip hitSound;
		[Tooltip("If greater than 0, pause the game momentarily when we get hit. This adds a lot of weight to impacts!")]
		public float hitPauseTime = 0;
		[Tooltip("How much do you want to vary the pitch of the hit sound?")]
		[Range(0, 1)]
		public float hitSoundVariance = .1f;
		[ReorderableAttribute]
		[Tooltip("What should we spawn when we die from HP loss?")]
		public GameObject[] deathPrefabs;

		[Header("GUI Settings")]
		[Tooltip("If using the Unity UI, create a slider for the health and drop a reference to it here. The slider value show the health amount. This can be used to create " +
			"either a health bar near the object in the scene, or displayed as a HUD. See the Unity GUI documentation/tutorials for more information. You can disable or reskin the handle so that " +
			"it doesn't look draggable.")]
		public Slider uIHealthBar;
		[Tooltip("A prefab with a Text component in it's heirarchy which will be used to display damage values. We recommend adding a Billboard component as well to ensure that it always faces the camera.")]
		public GameObject hitTextPrefab;
		[Tooltip("A spawn point where you want the hit text to appear (perhaps above the object?)")]
		public GameObject hitTextSpawnPoint;

		[Header("Immediate Mode GUI settings")]
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

		List<Armor> armors = new List<Armor>();
		float armorValue = 0;
		float hitStartingPitch = 1;
		float pauseTimer = 0;

		public HelpInfo help = new HelpInfo("This flexible Health implementation is fully compatible with all MultiGame systems and forms the basis for combat and death in your " +
			"game worlds. It receives the 'ModifyHealth' message with a floating point value. Positive numbers heal it, negative numbers damage it. Make anything killable! Fill out message senders above to " +
			"send messages to other MultiGame components.");

		public bool debug = false;

		
		void OnEnable () {
			hp = maxHP;
			if (hitAudioSource == null)
				hitAudioSource = GetComponent<AudioSource>();
			if (hitAudioSource == null)
				hitAudioSource = GetComponentInChildren<AudioSource>();
			if (hitAudioSource != null)
				hitStartingPitch = hitAudioSource.pitch;
			if (PlayerPrefs.HasKey ("Health" + autoSaveKey))
					hp = PlayerPrefs.GetFloat ("Health" + autoSaveKey);
			if (healthGoneMessage.target == null)
				healthGoneMessage.target = gameObject;
			if (hitMessage.target == null)
				hitMessage.target = gameObject;
			UpdateArmor();
		}

		void UpdateArmor() {
			armors.Clear();
			armors.AddRange(transform.root.GetComponentsInChildren<Armor>());
			armorValue = 0;
			foreach (Armor armor in armors)
				armorValue += armor.armorProtectionValue;
		}

		void OnDisable () {
			if (hitPauseTime > 0)
				Time.timeScale = 1f;
			if (!string.IsNullOrEmpty (autoSaveKey))
				PlayerPrefs.SetFloat ("Health" + autoSaveKey, hp);
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref healthGoneMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref hitMessage, gameObject);
		}
		
		void Update () {
			if (hp > maxHP)
				hp = maxHP;
			if (uIHealthBar != null) {
				uIHealthBar.maxValue = maxHP;
				uIHealthBar.minValue = 0;
				uIHealthBar.value = hp;
			}
		}
		
		void OnGUI () {
			//Debug.Log("" + GUI.contentColor);
			if (hp == maxHP && autoHide)
				return;
			if (showHealthBarGUI) {
				GUI.skin = guiSkin;
				GUI.contentColor = barColor;
				GUILayout.BeginArea(new Rect(healthBar.x * Screen.width, healthBar.y * Screen.height, (healthBar.width * Screen.width) * (hp / maxHP), healthBar.height * Screen.height));
				GUILayout.Box("Health");
				GUILayout.EndArea();
				
			}
		}

		void ReturnFromPool() {
			hp = maxHP;
		}

		[Header("Available Messages")]
		public MessageHelp hitStunHelp = new MessageHelp("HitStun","Stuns MultiGame AI and CharacterOmnicontroller components attached to this object",3,"How long should the stun last in seconds?");
		public void HitStun(float duration) {
			if (!gameObject.activeInHierarchy)
				return;
			gameObject.SendMessage("Stun", duration);
		}

		public MessageHelp dieHelp = new MessageHelp("Die","Kill this object immediately! Works even if the object is invulnerable");
		public void Die() {
			if (debug)
				Debug.Log("Health component " + gameObject.name + " has died! " + Time.timeScale);
			MessageManager.Send( healthGoneMessage);

			if (deathPrefabs.Length > 0) {
				for (int i = 0; i < deathPrefabs.Length; i++) {
					/*GameObject dFab = */
					Instantiate(deathPrefabs[i], transform.position, transform.rotation)/* as GameObject*/;
				}
			}
			if (!autodestruct)
				return;
			if (pool)
				gameObject.SetActive(false);
			else
				Destroy(gameObject);
		}

		public MessageHelp modifyHealthHelp = new MessageHelp("ModifyHealth","Change the health this object has", 3, "Amount to change the health by. Positive to increase, negative to decrease.");
		public void ModifyHealth (float val) {
			if (!gameObject.activeInHierarchy)
				return;
			if (hitTextPrefab != null) {
				GameObject textObject;
				if (hitTextSpawnPoint != null)
					textObject = Instantiate(hitTextPrefab, hitTextSpawnPoint.transform.position, hitTextSpawnPoint.transform.rotation);
				else
					textObject = Instantiate(hitTextPrefab,transform.position,transform.rotation);

				Text hitText = textObject.GetComponentInChildren<Text>();
				if (hitText != null) {
					hitText.text = ""+val;
				}
			}

			if (hitPrefab != null) {
				if (hitPrefabSpawnPoint != null)
					Instantiate(hitPrefab, hitPrefabSpawnPoint.transform.position, hitPrefabSpawnPoint.transform.rotation);
				else
					Instantiate(hitPrefab, transform.position, transform.rotation);
			}

			if (hitAudioSource != null && hitSound != null) {
				hitAudioSource.pitch = hitStartingPitch + Random.Range(-hitSoundVariance, hitSoundVariance);
				hitAudioSource.PlayOneShot(hitSound);
			}

			MessageManager.Send(hitMessage);
			if (val < 0) {
				float dmg = val;//-60
				dmg += armorValue;
				if (dmg < 0) {
					hp += dmg;
					if (debug)
						Debug.Log("Damaging " + gameObject.name + " by " + dmg);
				}
			}
			else {
				hp += val;
				if (debug)
					Debug.Log("Healing " + gameObject.name + " by " + val);
			}
			if (hp <= 0.0f && !invulnerable) {
				Die ();
				if (!autodestruct)
					return;
			}

			if (hitPauseTime > 0)
				StartCoroutine(HitPause());

		}

		IEnumerator HitPause() {
			pauseTimer = Time.realtimeSinceStartup + hitPauseTime;
			Time.timeScale = 0;
			while (Time.realtimeSinceStartup < pauseTimer)
				yield return 0;
			Time.timeScale = 1;
		}

		public MessageHelp modifyMaxHealthHelp = new MessageHelp("ModifyMaxHealth","Change the maximum health this object can have", 3, "Amount to change the max health by. Positive to increase, negative to decrease.");
		public void ModifyMaxHealth (float val) {
			if (debug)
				Debug.Log("Modifying health for " + gameObject.name + " by " + val);
			maxHP += val;
		}

		public MessageHelp makeVulnerableHelp = new MessageHelp("MakeVulnerable","Disables invulnerability on this object, allowing it do die if hp is less than or equal to 0");
		public void MakeVulnerable() {
			invulnerable = false;
		}

		public MessageHelp makeInvulnerableHelp = new MessageHelp("MakeInvulnerable","Prevents this object from dying even if it's health goes below 0. It can still be killed with the 'Die' message");
		public void MakeInvulnerable() {
			invulnerable = true;
		}

		public MessageHelp tempInvulnerabilityHelp = new MessageHelp("TempInvulnerability","Prevents this object from dying even if it's health goes below 0 for a limited duration. It can still be killed with the 'Die' message",3,"How long (in seconds) should the object be invulnerable?");
		public void TempInvulnerability(float duration) {
			invulnerable = true;
			StartCoroutine(ReMortalize(duration));
		}

		IEnumerator ReMortalize(float duration) {
			yield return new WaitForSeconds(duration);
			invulnerable = false;
		}
	}
}
//Copyright 2014 William Hendrickson
