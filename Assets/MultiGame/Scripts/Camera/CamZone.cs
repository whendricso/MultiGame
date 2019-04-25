using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Camera/Cam Zone")]
	public class CamZone : MultiModule {

		[Reorderable]
		[Header("Important - Must be populated")]
		public List<string> targetTags = new List<string>();
		[Reorderable]
		private List<GameObject> targets = new List<GameObject>();

		private Camera cam;
		private Collider coll;

		public HelpInfo help = new HelpInfo("This component allows you to have area cameras. We recommend setting up the camera to display in a corner or side of the screen, " +
			"but it can also be used for camera overlays. To use it, attach this to a collider, and parent a camera object to it. Position the camera to look into the collider " +
			"and when an object with a tag from the list enters, this camera will turn on.");

		// Use this for initialization
		void OnEnable () {
			if (coll == null)
				coll = GetComponent<Collider>();
			if (coll == null)
				coll = GetComponentInChildren<Collider>();
			if (cam == null)
				cam = GetComponentInChildren<Camera>();
			if (coll == null) {
				Debug.LogError("CamZone needs a collider to indicate the active zome.");
				gameObject.SetActive(false);
				return;
			}
			coll.isTrigger = true;
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

		void ReturnFromPool() {
			targets.Clear();
			cam.enabled = false;
		}
	}
}