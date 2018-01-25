using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Key Toggle")]
	public class KeyToggle : MultiModule {

		[Header("Toggle Targets")]
		[Tooltip("Game Objects we will toggle")]
		[ReorderableAttribute]
		public GameObject[] gameObjectTargets;//a list of targets to toggle
		[Tooltip("Scripts we will toggle")]
		[ReorderableAttribute]
		public MonoBehaviour[] scriptTargets;
		[Tooltip("Collider we will toggle")]
		public Collider colliderTarget;
		[Tooltip("Render we will toggle")]
		public MeshRenderer rendererTarget;
		[Header("Input Settings")]
		[Tooltip("Key to be pressed to toggle off")]
		public KeyCode off = KeyCode.LeftControl;
		[Tooltip("Key to be released to toggle on")]
		public KeyCode on = KeyCode.LeftControl;
		[Tooltip("Key to be tapped to swap the toggle")]
		public KeyCode swapKey = KeyCode.None;
		[Tooltip("Should we swap off/on?")]
		public bool reverse = false;
		[Tooltip("Are we currently toggled?")]
		public bool toggle = true;

		public HelpInfo help = new HelpInfo("This component allows objects, colliders, renderers, and scripts to be toggled based on the state of a given key. Off key is read on key up, On key is read on key down." +
			" This prevents key collisions when using the same key for both. If you want these objects and components to start in a 'Toggled off' state, set 'Toggle' to false, otherwise they will be enabled " +
			"automatically when the game starts. It may be more convenient to use this component than using a Message Toggle and Key Message in some situations.");
		
		void Start () {
			if (reverse) {
				foreach (GameObject gobj in gameObjectTargets) {
					gobj.SetActive(false);
				}
				foreach (MonoBehaviour behavior in scriptTargets) {
					behavior.enabled = toggle;
				}
				if (colliderTarget != null)
					colliderTarget.enabled = false;
				if (rendererTarget != null)
					rendererTarget.enabled = false;
			}
			else {
				foreach (GameObject gobj in gameObjectTargets) {
					gobj.SetActive(true);
				}
				foreach (MonoBehaviour behavior in scriptTargets) {
					behavior.enabled = toggle;
				}
				if (colliderTarget != null)
					colliderTarget.enabled = true;
				if (rendererTarget != null)
					rendererTarget.enabled = true;
			}
		}
		
		void Update () {
			if (Input.GetKeyDown(swapKey)) {
				toggle = !toggle;
				SwapToggles();
			}
			if (Input.GetKeyDown(off)) {
				if (!reverse)
					toggle = false;
				else
					toggle = true;
				SwapToggles();
			}
			if (Input.GetKeyUp(on)) {
				if (!reverse)
					toggle = true;
				else
					toggle = false;
				SwapToggles();
			}
		}
		
		void SwapToggles () {
			foreach (GameObject gobj in gameObjectTargets) {
				gobj.SetActive(toggle);
			}
			foreach (MonoBehaviour behavior in scriptTargets) {
				behavior.enabled = toggle;
			}
			if (colliderTarget != null)
				colliderTarget.enabled = toggle;
			if (rendererTarget != null)
				rendererTarget.enabled = toggle;
		}
	}
}