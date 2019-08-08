using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MultiGame;

namespace MultiGame {
	public class Hotbar : MultiModule {


		[Header("IMGUI Settings")]
		public bool showGui = true;
		public Rect guiArea = new Rect(.01f,.79f,.98f,.2f);
		public float maxButtonWidth = 64;

		[Header("General Settings")]
		public AudioSource source;
		public GameObject instantiationTransform;
		public string fileName;

		[Reorderable]
		public List<Action> actions = new List<Action>();

		[System.Serializable]
		public class Action {
			[Header("General Settings")]
			public string description;
			[Multiline]
			public string longDescription;
			public bool enabled = true;
			
			[Header("Charge and Cooldown")]
			[Tooltip("How long does it take to re-activate this action or gain another charge?")]
			public float cooldown = 0;
			[Tooltip("How many charges does the player start with?")]
			public int charges = 0;
			[Tooltip("How many charges can be held maximum? If zero, charges are ignored and the Action can be used as long as it's enabled.")]
			public int maxCharges = 0;

			[Header("Action Settings")]
			public MessageManager.ManagedMessage message;
			public GameObject spawnable;
			public AudioClip actionSound;
			
			public KeyCode key = KeyCode.None;
			[Header("IMGUI Settings")]
			public Texture2D icon;

			[Header("UGUI Settings")]
			public Button button;
			public Text text;
			public Slider cooldownSlider;

			//hidden members
			[System.NonSerialized]
			public float remainingCooldown = 0;

			public Action(int _charges, int _maxCharges, bool _enabled, float _remainingCooldown) {
				charges = _charges;
				maxCharges = _maxCharges;
				enabled = _enabled;
				remainingCooldown = _remainingCooldown;
			}
		}

		void OnValidate() {
			foreach (Action _action in actions) {
				MessageManager.UpdateMessageGUI(ref _action.message, gameObject);
				if (_action.message.target == null)
					_action.message.target = gameObject;
			}
		}

		void Awake() {
			if (source == null)
				source = GetComponentInChildren<AudioSource>();

			if (instantiationTransform == null)
				instantiationTransform = gameObject;

			foreach (Action _action in actions) {
				_action.remainingCooldown = 0;
				if (_action.cooldown < 0)
					_action.cooldown = 0;
			}
		}

		private void OnGUI() {
			if (!showGui)
				return;
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), "", "box");


			GUILayout.BeginHorizontal();
			bool currentActionActive = false;
			for (int i = 0; i < actions.Count; i++) {
				GUILayout.BeginVertical();

				if ((actions[i].maxCharges > 0 && actions[i].charges > 0) || actions[i].remainingCooldown <= 0) {
					GUI.color = Color.white;
					currentActionActive = true;
				} else {
					GUI.color = Color.gray;
					currentActionActive = false;
				}

				if (currentActionActive) {
					if (GUILayout.Button(GenerateButtonContent(actions[i]),GUILayout.Width(maxButtonWidth), GUILayout.Height(maxButtonWidth) )) {
						ActivateAction(i);
					}
				} else {
					GUILayout.Box(GenerateButtonContent(actions[i]), GUILayout.Width(maxButtonWidth), GUILayout.Height(maxButtonWidth));
				}
				#region oldGUI
				/*if (actions[i].enabled && actions[i].remainingCooldown <= 0) {
					if (actions[i].icon != null) {
						if (GUILayout.Button(actions[i].icon, GUILayout.MaxWidth(maxButtonWidth), GUILayout.MaxHeight(maxButtonWidth)))
							ActivateAction(i);
					} else {
						if (GUILayout.Button(string.IsNullOrEmpty(actions[i].description) ? "" + (actions[i].key == KeyCode.None ? "" : "" + actions[i].key) :  (actions[i].key == KeyCode.None ? "" : "[" + actions[i].key + "] ")  + actions[i].description, GUILayout.MaxWidth(maxButtonWidth), GUILayout.MaxHeight(maxButtonWidth)))
							ActivateAction(i);
					}
				} else {
					GUI.color = Color.gray;
					if (actions[i].icon != null) {
						GUILayout.Box(actions[i].icon, GUILayout.MaxWidth(maxButtonWidth), GUILayout.MaxHeight(maxButtonWidth));
					} else {
						GUILayout.Box(string.IsNullOrEmpty(actions[i].description) ? "" + (actions[i].key == KeyCode.None ? "" : "" + actions[i].key) : "[" + (actions[i].key == KeyCode.None ? "" : "" + actions[i].key) + "] " + actions[i].description, GUILayout.MaxWidth(maxButtonWidth), GUILayout.MaxHeight(maxButtonWidth));
					}
				}
				*/
				//GUILayout.Label(string.IsNullOrEmpty(actions[i].description) ? "" + (actions[i].key == KeyCode.None ? "" : "" + actions[i].key) : "[" + (actions[i].key == KeyCode.None ? "" : "" + actions[i].key) + "] " + actions[i].description);
				#endregion
				GUI.color = Color.white;
				if (actions[i].maxCharges > 0)
					GUILayout.Label(actions[i].charges + "/" + actions[i].maxCharges);
				GUILayout.EndVertical();
			}

			GUILayout.EndHorizontal();

			GUILayout.Label(" "+GUI.tooltip, "box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			GUILayout.EndArea();
		}

		private GUIContent GenerateButtonContent(Action _action) {
			GUIContent _ret = null;
			string buttonText = "";

			buttonText = buttonText + (_action.key == KeyCode.None ? "" : "[" + _action.key + "]");
			buttonText = buttonText + (string.IsNullOrEmpty( _action.description) ? "" : " " + _action.description);
			buttonText = buttonText + (_action.maxCharges > 0 ? " " + _action.charges + "/" + _action.maxCharges : "");
			buttonText = buttonText + (string.IsNullOrEmpty(_action.longDescription) ? "" : _action.longDescription);

			if (_action.icon != null)
				_ret = new GUIContent(_action.icon, buttonText);
			else
				_ret = new GUIContent(_action.description, string.IsNullOrEmpty( _action.longDescription) ? buttonText : _action.longDescription);//tooltip
			return _ret;
		}

		void Update() {
			for(int i = 0; i < actions.Count; i++) {
				actions[i].remainingCooldown -= Time.deltaTime;
				if (actions[i].maxCharges > 0) {//max charge count is defined, so use charge/cooldown behavior instead of strict cooldown
					if (actions[i].charges < actions[i].maxCharges) {
						if (actions[i].remainingCooldown < 0) {
							actions[i].remainingCooldown = actions[i].cooldown;
							actions[i].charges++;
						}
					} else {//we have max charges, so the cooldown is fixed to it's maximum value
						actions[i].remainingCooldown = actions[i].cooldown;
					}
				}

				//take keyboard input
				if (Input.GetKeyDown(actions[i].key) && actions[i].enabled) {
					ActivateAction(i);
				}

				//update UGUI
				if (actions[i].cooldownSlider != null) {
					if (actions[i].remainingCooldown > 0 && actions[i].cooldown > 0) {
						actions[i].cooldownSlider.value = (actions[i].remainingCooldown/actions[i].cooldown);
					}
				}

			}
		}

		private void ExecuteSelectedAction(Action _action) {
			if (_action.maxCharges <= 0)
				_action.remainingCooldown = _action.cooldown;
			else
				_action.charges--;
			MessageManager.Send(_action.message);
			if (_action.spawnable != null)
				Instantiate(_action.spawnable, instantiationTransform.transform.position, instantiationTransform.transform.rotation);
			if (source != null && _action.actionSound != null)
				source.PlayOneShot(_action.actionSound);
		}

		public MessageHelp addChargeHelp = new MessageHelp("AddCharge","Adds a charge to the selected action, or refreshes it's cooldown instantly.",2, "The index of the action we wish to recharge. These are zero-indexed, so the first Action in the list is 0, the second is 1 and so forth.");
		public void AddCharge(int _selector) {
			if (actions[_selector].maxCharges > 0 && actions[_selector].charges < actions[_selector].maxCharges)
				actions[_selector].charges++;
			if (actions[_selector].maxCharges <= 0)
				actions[_selector].cooldown = 0;
		}

		public MessageHelp activateActionHelp = new MessageHelp("ActivateAction","Fires an action from the list if it is off of cooldown or has charges available",2,"The index of the action we wish to execute. These are zero-indexed, so the first Action in the list is 0, the second is 1 and so forth.");
		public void ActivateAction(int _selector) {
			if (actions[_selector].maxCharges > 0) {
				if (actions[_selector].charges > 0)
					ExecuteSelectedAction(actions[_selector]);
			} else {
				if (actions[_selector].remainingCooldown <= 0) {
					ExecuteSelectedAction(actions[_selector]);
				}
			}
		}

		public MessageHelp saveHelp = new MessageHelp("Save","Saves the Hotbar to PlayerPrefs based on it's unique File Name, defined above. To save multiple Hotbars, give each a different File Name");
		public void Save() {
			if (string.IsNullOrEmpty(fileName))
				return;
			if (actions.Count <= 0)
				return;
			for (int i = 0; i < actions.Count; i++) {
				PlayerPrefs.SetInt("Hotbar_" + fileName + "charges_" + i,actions[i].charges);
				PlayerPrefs.SetInt("Hotbar_" + fileName + "maxCharges_" + i,actions[i].charges);
				PlayerPrefs.SetInt("Hotbar_" + fileName + "enabled_" + i,(actions[i].enabled ? 1 : 0));
				PlayerPrefs.SetFloat("Hotbar_" + fileName + "cooldown_" + i,actions[i].cooldown);
				PlayerPrefs.SetFloat("Hotbar_" + fileName + "remainingCooldown_" + i,actions[i].remainingCooldown);
			}
		}


		public MessageHelp loadHelp = new MessageHelp("Load","Loads the Hotbar from PlayerPrefs based on it's unique File Name, defined above. To load multiple Hotbars, give each a different File Name");
		public void Load() {
			if (string.IsNullOrEmpty(fileName))
				return;
			if (actions.Count <= 0)
				return;
			if (!PlayerPrefs.HasKey("Hotbar_" + fileName + "charges_" + "0"))
				return;
			
			for (int i = 0; i < actions.Count; i++) {
				if (PlayerPrefs.HasKey("Hotbar_" + fileName + "charges_" + i)) {
					actions[i].charges = PlayerPrefs.GetInt("Hotbar_" + fileName + "charges_" + i);
					actions[i].maxCharges = PlayerPrefs.GetInt("Hotbar_" + fileName + "maxCharges_" + i);
					actions[i].enabled = (PlayerPrefs.GetInt("Hotbar_" + fileName + "enabled_" + i) == 1);
					actions[i].cooldown = PlayerPrefs.GetFloat("Hotbar_" + fileName + "cooldown_" + i);
					actions[i].remainingCooldown = PlayerPrefs.GetFloat("Hotbar_" + fileName + "remainingCooldown_" + i);
				}
			}
		}
	}
}