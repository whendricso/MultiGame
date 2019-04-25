using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Character/Character Selector")]
	public class CharacterSelector : MultiModule {
		
		[Header("GUI Settings")]
		[Tooltip("Should we show the GUI right now?")]
		public bool showGUI = false;
		[Tooltip("Normalized viewport rectangle indicating the button area. values between 0 and 1")]
		public Rect guiArea = new Rect(0.3f,0.3f,0.3f,0.3f);
		[Tooltip("Allow this script to control the cursor?")]
		public bool toggleCursor = false;
		[Tooltip("Should the camera be destroyed when switching characters?")]
		public bool destroyCamera = true;
		[RequiredFieldAttribute("If greater than 0, close the GUI if the player leaves this radius. Assumes that the player is tagged 'Player'", RequiredFieldAttribute.RequirementLevels.Optional)]
		public float maxDistance = 0.0f;
		[RequiredFieldAttribute("Unique window ID number for this window, must not conflict with any others (change it if it does!)")]
		public int windowID = 92829;
		
		[Header("Spawn Settings")]
		[RequiredFieldAttribute("Where do we spawn the new player object?", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject spawnPoint;
		[ReorderableAttribute]
		[Tooltip("What characters can we select from?")]
		public List<GameObject> characters;


		public HelpInfo help = new HelpInfo("NOTE: Character Selector implements character selection using the legacy Unity GUI. Not suitable for mobile." +
			"\n\n" +
			"Provide a list of characters, and send a message to this object when it's time for character selection (ToggleOn & ToggleOff) to show the menu. This allows the player to switch characters " +
			"mid-game");

		void Start () {
			if (spawnPoint == null)
				spawnPoint = gameObject;
		}

		void OnGUI () {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null && maxDistance > 0.0f && showGUI) {
				if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
					ToggleOff();
			}
				
			if (showGUI)
				GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), CharacterMenu,"Character Select", "window", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}
				
		void CharacterMenu (int id) {
			if (GUILayout.Button("Cancel"))
				Toggle(false);
			for (int i = 0; i < characters.Count; i++) {
				if (GUILayout.Button(characters[i].name))
					StartCoroutine(SwapCharacter(characters[i]));
			}
		}
		
		IEnumerator SwapCharacter (GameObject character) {
			Destroy(GameObject.FindGameObjectWithTag("Player"));
			if (destroyCamera)
				Destroy(Camera.main.gameObject);
			yield return new WaitForEndOfFrame();

			Instantiate(character, spawnPoint.transform.position, spawnPoint.transform.rotation);

		}

		[Header("Available Messages")]
		public MessageHelp toggleOnHelp = new MessageHelp("ToggleOn","Enables the character selection GUI");
		public void ToggleOn() {
			if (!gameObject.activeInHierarchy)
				return;
			Toggle(true);
		}
		
		public MessageHelp toggleOffHelp = new MessageHelp("ToggleOff","Disables the character selection GUI");
		public void ToggleOff() {
			if (!gameObject.activeInHierarchy)
				return;
			Toggle(false);
		}
		
		public MessageHelp toggleHelp = new MessageHelp("Toggle","Toggles the character selection GUI based on a supplied value", 1, "Should the character selection GUI be shown?");
		void Toggle (bool val) {
			if (!gameObject.activeInHierarchy)
				return;
			if (toggleCursor) {
				if (val) {
					Cursor.lockState = CursorLockMode.Locked;//Screen.lockCursor = !val;
					Cursor.visible = false;
				}
				else {
					Cursor.lockState = CursorLockMode.Confined;
					Cursor.visible = true;
				}
			}
			showGUI = val;
		}
	//Copyright 2013-2016 William Hendrickson
	}
}