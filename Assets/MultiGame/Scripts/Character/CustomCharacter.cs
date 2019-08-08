using UnityEngine;
using System.Collections;
using MultiGame;
using System.Collections.Generic;
//using System.IO;

namespace MultiGame {

	public class CustomCharacter : MultiModule {

		[Header("IMGUI Settings")]
		[Tooltip("Should we use the built-in GUI? Not suitable for mobile devices.")]
		public bool showGui = true;
		[Tooltip("an optional skin to use for the GUI")]
		public GUISkin guiSkin;
		[Space]
		[Tooltip("Normalized viewport rectangle indicating the area where the GUI will appear. Values must be between 0 and 1 and indicate a percentage of screen space")]
		public Rect guiArea = new Rect(0.3f,0.6f,.3f,.3f);
		[Space]
		[Tooltip("Normalized viewport rectangle indicating the area where the Load window will appear. Values must be between 0 and 1 and indicate a percentage of screen space")]
		public Rect loadWindow = new Rect(0.61f,0.6f,.3f,.3f);
		[Space]
		[Tooltip("A unique ID for the load character window, if used. Must be different from any other window ID in your game.")]
		public int windowID = 94556;
		[RequiredFieldAttribute("The default name for new characters.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string characterName = "Player";

		[ReorderableAttribute]
		[Header("Object Settings")]
		[Tooltip("Prefabs that can be used as a base model. Must match the prefab name exactly. The prefab must be directly inside a 'Resources' folder.")]
		public List<string> baseModels = new List<string>();
		private int currentBaseModel = 0;
		[HideInInspector]
		public GameObject character;
		[ReorderableAttribute]
		public List<AttachableSlot> attachableSlots = new List<AttachableSlot>();
//		[HideInInspector]
//		public List<GameObject> attachedObjects = new List<GameObject>();

		private bool showLoadWindow = false;
		private int numCharactersSaved = 0;
		private List<string> savedCharacterNames = new List<string>();
//		private string saveString = "";

		private Vector2 scrollArea = Vector2.zero;


		public HelpInfo help = new HelpInfo("Custom Character allows the player to create a custom character by selecting prefabs and attaching cosmetics. It works by searching 'Resources' " +
			"folders inside your project for the Prefab names you specify. This allows it to load the same Prefabs across multiple clients. " +
			"To use this component, first create some basic character prefabs, name them uniquely and place inside a 'Resources' folder in your Project. Then, make some accessories and also place them in " +
			"another 'Resources' folder. Add the basic character prefab names to the 'Base Models' list. Find or create some transforms on the character, where 'Attachable Slots' could be (for example a spot " +
			"for a hat), name these uniquely and add their names to the 'Attachable Slots' list." +
			"\n\n" +
			"Takes 'Save' 'Load' which takes a string matching the character's name, 'EditCharacter' and messages to toggle the immediate mode Unity GUI. The built-in GUI does not work on mobile devices. " +
			"Player data is saved inside of PlayerPrefs, so if PlayerPrefs are cleared, all characters will be lost! Be sure to get user confirmation *BEFORE* clearing PlayerPrefs!");

		public bool debug = false;

		[System.Serializable]
		public class AttachableSlot {
			[Tooltip("Name of this attachable slot, shown to the player.")]
			public string name = "";
			[Tooltip("Name of the bone or transform we can attach this cosmetic to. Must match the name in the Heirarchy exactly.")]
			public string heirarchyTransform;
			[Tooltip("Prefabs that can be attached to the character in this location. Must match the prefab name exactly, and the prefab must be " +
				"directly inside a 'Resources' folder in your Project.")]
			public List<string> possibleAttachables = new List<string>();
			[HideInInspector]
			public GameObject currentAttachment;
		}

		void Start () {
			SelectCharacter(0);
			if (PlayerPrefs.HasKey("numCharactersSaved")) {
				numCharactersSaved = PlayerPrefs.GetInt("numCharactersSaved");
				savedCharacterNames.Clear();
				for (int i = 0; i < numCharactersSaved; i++) {
					savedCharacterNames.Add(PlayerPrefs.GetString("savedCharacterName" + i));
				}
			}
		}

		void OnGUI() {
			if (!showGui)
				return;

			if (showLoadWindow) {
				GUILayout.Window(windowID, new Rect(Screen.width * loadWindow.x, Screen.height * loadWindow.y, Screen.width * loadWindow.width, Screen.height * loadWindow.height), LoadWindow, "Load");
			}

			GUI.skin = guiSkin;

			GUILayout.BeginArea(new Rect(Screen.width * guiArea.x, Screen.height * guiArea.y, Screen.width * guiArea.width, Screen.height * guiArea.height), "Character Creator","box");
			scrollArea = GUILayout.BeginScrollView(scrollArea);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Name: ");
			characterName = GUILayout.TextField(characterName);
			GUILayout.EndHorizontal();

			GUILayout.Label("Character:");
			for (int i = 0; i < baseModels.Count; i++) {
				if (GUILayout.Button(baseModels[i])) {
					SelectCharacter(i);
				}
			}

			GUILayout.Label("Accessories:");
			for (int j = 0; j < attachableSlots.Count; j++) {
				GUILayout.Label(attachableSlots[j].name + ":");
				for (int k = 0; k < attachableSlots[j].possibleAttachables.Count; k++) {
					if (GUILayout.Button(attachableSlots[j].possibleAttachables[k])) {
						AttachToSlot(j,k);
					}
				}
			}

			GUILayout.FlexibleSpace();


			if (GUILayout.Button("Close")) {
				CloseMenu();
				EnableControls();
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save")) {
				CloseMenu();
				EnableControls();
				Save();
			}
			if (GUILayout.Button("Load")) {
				numCharactersSaved = PlayerPrefs.GetInt("numCharactersSaved");
				savedCharacterNames.Clear();
				for (int i = 0; i < numCharactersSaved; i++) {
					savedCharacterNames.Add(PlayerPrefs.GetString("savedCharacterName" + i));
				}
				showLoadWindow = true;
			}

			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		private void LoadWindow (int _id) {
			for (int i = 0; i < numCharactersSaved; i++) {
				if (GUILayout.Button(savedCharacterNames[i])) {
					Load(savedCharacterNames[i]);
					showLoadWindow = false;
					CloseMenu();
					EnableControls();
				}
			}
			if (GUILayout.Button("Cancel"))
				showLoadWindow = false;
		}

		public void SelectCharacter (int _selection) {
			currentBaseModel = _selection;
			if (character != null)
				Destroy(character);
			character = Instantiate( Resources.Load(baseModels[_selection]), transform.position, transform.rotation) as GameObject;
			character.AddComponent<CloneFlagRemover>();
			DisableControls();
		}

		public void AttachToSlot (int _slot, int _selection) {
			if (character == null) {
				if (debug)
					Debug.LogError("Custom Character " + gameObject.name + " does not have a character to attach objects to.");
				return;
			}
			if (attachableSlots[_slot] == null) {
				if (debug)
					Debug.LogError ("Custom Character " + gameObject.name + " could not find slot " + _slot + " on character " + character.name );
				return;
			}

			if ( attachableSlots[_slot].currentAttachment != null)
				Destroy(attachableSlots[_slot].currentAttachment);

			attachableSlots[_slot].currentAttachment = Instantiate(Resources.Load( attachableSlots[_slot].possibleAttachables[_selection])) as GameObject;
			attachableSlots[_slot].currentAttachment.AddComponent<CloneFlagRemover>();
			attachableSlots[_slot].currentAttachment.transform.SetParent(FindChildRecursive(character, attachableSlots[_slot].heirarchyTransform).transform);
			attachableSlots[_slot].currentAttachment.transform.localPosition = Vector3.zero;
			attachableSlots[_slot].currentAttachment.transform.localRotation = Quaternion.identity;
		}

		[Header("Available Messages")]
		public MessageHelp disableControlsHelp = new MessageHelp("DisableControls","Disables control of the character without affecting the menu.");
		public void DisableControls() {
			if (!gameObject.activeInHierarchy)
				return;
			if (character == null) {
				Debug.LogError("Character object must be instantiated before controls can be disabled.");
				return;
			}

			CharacterController _ctrl = character.GetComponent<CharacterController>();
			if (_ctrl != null)
				_ctrl.enabled = false;

			MonoBehaviour _cmp;
			_cmp = character.GetComponent<MouseAim>();
			if (_cmp != null)
				_cmp.enabled = false;
			
			_cmp = character.GetComponent<CharacterOmnicontroller>();
			if (_cmp != null)
				_cmp.enabled = false;

			_cmp = character.GetComponent<CharacterInputAnimator>();
			if (_cmp != null)
				_cmp.enabled = false;
		}

		public MessageHelp enableControlsHelp = new MessageHelp("EnableControls","Enables control of the character without affecting the menu.");
		public void EnableControls() {
			if (!gameObject.activeInHierarchy)
				return;
			if (character == null) {
				Debug.LogError("Character object must be instantiated before controls can be enabled.");
				return;
			}

			CharacterController _ctrl = character.GetComponent<CharacterController>();
			if (_ctrl != null)
				_ctrl.enabled = true;

			MonoBehaviour _cmp;
			_cmp = character.GetComponent<MouseAim>();
			if (_cmp != null)
				_cmp.enabled = true;

			_cmp = character.GetComponent<CharacterOmnicontroller>();
			if (_cmp != null)
				_cmp.enabled = true;

			_cmp = character.GetComponent<CharacterInputAnimator>();
			if (_cmp != null)
				_cmp.enabled = true;
		}

		public void Save () {
			if (string.IsNullOrEmpty( characterName)) {
				if (debug)
					Debug.LogError("Tried to save, but the character name is empty!");
				return;
			}
			PlayerPrefs.SetInt(characterName + "currentBaseModel", currentBaseModel);
			PlayerPrefs.SetInt(characterName + "attachableSlotsCount", attachableSlots.Count);
//			PlayerPrefs.SetInt(characterName + "attachedObjectsCount", attachedObjects.Count);
			for (int i = 0; i < attachableSlots.Count; i++) {
				if (attachableSlots[i].currentAttachment != null)
					PlayerPrefs.SetString(characterName + "attachment" + i, attachableSlots[i].currentAttachment.name);
			}
//			for (int j = 0; j < attachedObjects.Count; j++) {
//				PlayerPrefs.SetString(characterName + "attachedObject" + j, attachedObjects[j].name);
//			}
			if (!savedCharacterNames.Contains(characterName))
				savedCharacterNames.Add(characterName);
			numCharactersSaved = savedCharacterNames.Count;
			PlayerPrefs.SetInt("numCharactersSaved", numCharactersSaved);
			for (int k = 0; k < numCharactersSaved; k++) {
				PlayerPrefs.SetString("savedCharacterName" + k, savedCharacterNames[k]);
			}
			PlayerPrefs.Save();
			if (debug)
				Debug.Log("Custom Character " + gameObject.name + " saved " + characterName + " successfully.");
		}



		public void Load (string _characterName) {
			if (string.IsNullOrEmpty(_characterName)) {
				if (debug)
					Debug.LogError("Custom Character " + gameObject.name + " tried to load but the character name was empty! Please pass a string to 'Load' that matches the character name");
				return;
			}
			characterName = _characterName;
			if (!PlayerPrefs.HasKey(_characterName + "currentBaseModel")) {
				if (debug)
					Debug.LogError("Custom Character " + gameObject.name + " could not find a character save named " + _characterName);
				return;
			}
			SelectCharacter(PlayerPrefs.GetInt(characterName + "currentBaseModel"));
			int _slots = PlayerPrefs.GetInt(characterName + "attachableSlotsCount");
//			int _objects = PlayerPrefs.GetInt(characterName + "attachedObjectsCount");
			for (int i = 0; i < _slots; i++) {
				if (PlayerPrefs.HasKey(characterName + "attachment" + i)) {
					if ( attachableSlots[i].currentAttachment != null)
						Destroy(attachableSlots[i].currentAttachment);

					attachableSlots[i].currentAttachment = Instantiate(Resources.Load( PlayerPrefs.GetString(characterName + "attachment" + i))) as GameObject;
					attachableSlots[i].currentAttachment.AddComponent<CloneFlagRemover>();
					attachableSlots[i].currentAttachment.transform.SetParent(FindChildRecursive(character, attachableSlots[i].heirarchyTransform).transform);
					attachableSlots[i].currentAttachment.transform.localPosition = Vector3.zero;
					attachableSlots[i].currentAttachment.transform.localRotation = Quaternion.identity;
				}
			}
			EnableControls();
		}

		public MessageHelp editCharacterHelp = new MessageHelp("EditCharacter","Enables character editing and disables controls for a clean experience.");
		public void EditCharacter () {
			OpenMenu();
			DisableControls();
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Opens the character editor without toggling controls.");
		public void OpenMenu() {
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Closes the character editor without toggling controls.");
		public void CloseMenu() {
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toggles the character editor without toggling controls.");
		public void ToggleMenu() {
			showGui = !showGui;
		}

	}
}






















