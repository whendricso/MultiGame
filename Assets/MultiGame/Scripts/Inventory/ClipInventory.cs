using UnityEngine;
using System.Collections;

public class ClipInventory : MonoBehaviour {
	
	public int[] numClips;
	public int[] maxClips;
	public bool useGUI = true;
	public Rect guiArea;

	public bool autoSave = true;
	
	void Start () {
		if (maxClips.Length != numClips.Length) {
			Debug.LogError("Clip Inventory must have an equal number of Num Clips and Max Clips assigned in the inspector.");
			enabled = false;
			return;
		}
		int _numCliTypes;
		if(autoSave && PlayerPrefs.HasKey("clipTypeCount")) {
			_numCliTypes = PlayerPrefs.GetInt ("clipTypeCount");
			numClips = new int[_numCliTypes];
			for (int i = 0; i < _numCliTypes; i++) {
				numClips[i] = PlayerPrefs.GetInt("numClips" + i);
			}
		}
	}
	
	void OnGUI () {
		if (!useGUI)
			return;
		
		GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");
		for (int i = 0; i < numClips.Length; i ++) {
			GUILayout.Label(numClips[i] + " : " + maxClips[i]);
		}
		GUILayout.EndArea();
	}


}