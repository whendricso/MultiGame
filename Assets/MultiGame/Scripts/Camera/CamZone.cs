using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Camera/Cam Zone")]
	public class CamZone : MultiModule {

		public HelpInfo help = new HelpInfo("This component allows you to have area cameras. We recommend setting up the camera to display in a corner or side of the screen, " +
			"but it can also be used for camera overlays. To use it, attach this to a collider, and parent a camera object to it. Position the camera to look into the collider " +
			"and when the object tagged as \"Player\" enters, this camera will turn on.");

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
}