using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public string firstLevel;
	public Rect menuArea;
	
	void OnGUI () {
		GUILayout.BeginArea(new Rect (menuArea.x * Screen.width, menuArea.y * Screen.height, menuArea.width * Screen.width, menuArea.height * Screen.height), "", "box");
		if (GUILayout.Button("New Game"))
			Application.LoadLevel(firstLevel);
		if (PlayerPrefs.HasKey("continue")) {
			if (GUILayout.Button("Continue"))
				Application.LoadLevel(PlayerPrefs.GetInt("continue"));
		}
		if (GUILayout.Button("Quit"))
			Application.Quit();
		GUILayout.EndArea();
	}
	
	
}
