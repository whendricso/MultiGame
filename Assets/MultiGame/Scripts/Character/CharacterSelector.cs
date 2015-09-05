using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelector : MultiModule {
	
	[Tooltip("Normalized viewport rectangle indicating the button area. values between 0 and 1")]
	public Rect guiArea = new Rect(0.3f,0.3f,0.3f,0.3f);
	[Tooltip("Should the camera be destroyed when switching characters?")]
	public bool destroyCamera = true;
	[Tooltip("What characters can we select from?")]
	public List<GameObject> characters;
	[Tooltip("Allow this script to control the cursor?")]
	public bool toggleCursor = false;
	
	[Tooltip("If greater than 0, close the GUI if the player leaves this radius")]
	public float maxDistance = 0.0f;
	[Tooltip("Unique window ID number for this window, must not conflict with any others (change it if it does!)")]
	public int windowID = 92829;
	
	[Tooltip("Where do we spawn the new player object?")]
	public GameObject spawnPoint;
	
	[Tooltip("Should we show the GUI right now?")]
	public bool showGUI = false;

	public HelpInfo help = new HelpInfo("This component implements character selection using the legacy Unity GUI. Not suitable for mobile. Provide a list of characters, " +
		"and send a message to this object when it's time for character selection (ToggleOn & ToggleOff)");
	
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
	
	public void ToggleOn() {
		Toggle(true);
	}
	
	public void ToggleOff() {
		Toggle(false);
	}
	
	void Toggle (bool val) {
		if (toggleCursor)
			Screen.lockCursor = !val;
		showGUI = val;
	}
//Copyright 2014 William Hendrickson
}
