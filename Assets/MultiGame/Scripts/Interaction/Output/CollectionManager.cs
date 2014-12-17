using UnityEngine;
using System.Collections;

public class CollectionManager : MonoBehaviour {

	public Rect guiArea;
	public string windowTitle = "";
	public int windowID = 53346;
	public int max = 0;
	public string collectionMessage = "";
	public GameObject target;
	public bool closeOnCompletion = false;

	private bool showGUI = true;
	[System.NonSerialized]
	public int collected = 0;


	void Start () {
		if (target == null)
			target = gameObject;
		if (windowTitle == "")
			windowTitle = "Collect";
	}

	void OnGUI () {
		if (!showGUI)
			return;
		GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), CollectionWindow, windowTitle);
	}

	void Collect () {
		collected ++;
		if (collected >= max) {
			if( collectionMessage != "")
				target.SendMessage(collectionMessage, SendMessageOptions.DontRequireReceiver);
			if (closeOnCompletion)
				showGUI = false;
		}
	}

	void CollectionWindow (int id) {
		if (max > 0)
			GUILayout.Label("" + collected + "/" + max);
		else
			GUILayout.Label("" + collected);
	}
}
