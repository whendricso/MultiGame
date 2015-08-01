using UnityEngine;
using System.Collections;

public class CamZone : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		if (GetComponent<Collider>() == null) {
			Debug.LogError("CamZone needs a collider to indicate the active zome.");
			gameObject.SetActive(false);
			return;
		}
		GetComponent<Collider>().isTrigger = true;
		if (GetComponentInChildren<Camera>() == null) {	
			Debug.LogError("CamZone needs a camera in one of it's child objects.");
			gameObject.SetActive(false);
			return;
		}
		GetComponentInChildren<Camera>().enabled = false;
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player")
			GetComponentInChildren<Camera>().enabled = true;
	}
	
	void OnTriggerExit (Collider other) {
		if (other.gameObject.tag == "Player")
			GetComponentInChildren<Camera>().enabled = false;
	}
	
}
