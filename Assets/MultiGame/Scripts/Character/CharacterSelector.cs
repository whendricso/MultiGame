using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour {
	
	public Rect guiArea;
	public bool destroyCamera = true;
	public List<GameObject> characters;
	public bool toggleCursor = false;
	
	public float maxDistance = 0.0f;
	public int windowID = 92829;
	
	public GameObject spawnPoint;
	
	public bool showGUI = false;
	
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
	
	void ToggleOn() {
		Toggle(true);
	}
	
	void ToggleOff() {
		Toggle(false);
	}
	
	void Toggle (bool val) {
		if (toggleCursor)
			Screen.lockCursor = !val;
		showGUI = val;
	}
//Copyright 2014 William Hendrickson
}
