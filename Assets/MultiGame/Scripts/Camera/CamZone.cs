using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Camera/Cam Zone")]
	public class CamZone : MultiModule {

		public List<string> targetTags = new List<string>();

		private List<GameObject> targets = new List<GameObject>();

		private Camera cam;

		public HelpInfo help = new HelpInfo("This component allows you to have area cameras. We recommend setting up the camera to display in a corner or side of the screen, " +
			"but it can also be used for camera overlays. To use it, attach this to a collider, and parent a camera object to it. Position the camera to look into the collider " +
			"and when an object with a tag from the list enters, this camera will turn on.");

		// Use this for initialization
		void Start () {
			cam = GetComponentInChildren<Camera>();
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
			cam.enabled = false;
		}
		
		void OnTriggerEnter (Collider other) {
			if (targetTags.Contains( other.gameObject.tag) && !targets.Contains(other.gameObject)) {
				targets.Add(other.gameObject);
			}
			if (targets.Count > 0)
				cam.enabled = true;
		}
		
		void OnTriggerExit (Collider other) {
			if (targets.Contains( other.gameObject)) {
				targets.Remove(other.gameObject);
			}
			if (targets.Count < 1)
				cam.enabled = false;
		}
		
	}
}