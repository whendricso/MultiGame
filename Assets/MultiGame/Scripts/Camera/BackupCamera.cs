using UnityEngine;
using System.Collections;

public class BackupCamera : MonoBehaviour {

	private AudioListener listener;

	void Start () {
		if (GetComponent<Camera>() == null) {
			Debug.LogError ("Backup Camera " + gameObject.name + " needs a camera component attached to work! Disabling...");
			enabled = false;
			return;
		}
		listener = GetComponent<AudioListener>();
	}
	
	void Update () {
		if (listener != null) {
			if (Camera.main != null)
				listener.enabled = false;
			else
				listener.enabled = true;
		}
		if (Camera.main != null)
			GetComponent<Camera>().enabled = false;
		else
			GetComponent<Camera>().enabled = true;
	}
}
