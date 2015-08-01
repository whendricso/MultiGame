using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentDataSaveGameManager : MonoBehaviour {

	public Rect guiArea = new Rect();
	public int windowID = 7848324;


	private bool confirmation = false;
	
	void OnGUI () {
		if (!(guiArea.width > 0f))
			return;
		
		GUILayout.Window(windowID, guiArea, SaveWindow,"Saved Game Data");
	}
	
	private void SaveWindow (int _id) {
		if (!confirmation) {
			if (GUILayout.Button("Clear all save game data?"))
				confirmation = true;
		}
		else {
			if (GUILayout.Button("Wait, NO!!!"))
				confirmation = false;
			if (GUILayout.Button("Really clear all data!"))
				ClearSavedGame();
		}
	}
	
	public void ClearSavedGame () {

	}
}
