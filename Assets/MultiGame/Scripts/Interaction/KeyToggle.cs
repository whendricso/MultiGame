using UnityEngine;
using System.Collections;

public class KeyToggle : MultiModule {
	
	[Tooltip("Game Objects we will toggle")]
	public GameObject[] gameObjectTargets;//a list of targets to toggle
	[Tooltip("Scripts we will toggle")]
	public MonoBehaviour[] scriptTargets;
	[Tooltip("Collider we will toggle")]
	public Collider colliderTarget;
	[Tooltip("Render we will toggle")]
	public MeshRenderer rendererTarget;
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

	public HelpInfo help = new HelpInfo("This component allows objects, colliders, renderers, and scripts to be toggled based on the state of a given key.");
	
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
