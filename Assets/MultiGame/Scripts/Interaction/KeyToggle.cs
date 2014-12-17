using UnityEngine;
using System.Collections;

public class KeyToggle : MonoBehaviour {
	
	public GameObject[] gameObjectTargets;//a list of targets to toggle
	public MonoBehaviour[] scriptTargets;
	public Collider colliderTarget;
	public MeshRenderer rendererTarget;
	public KeyCode off = KeyCode.LeftControl;
	public KeyCode on = KeyCode.LeftControl;
	public KeyCode swapKey = KeyCode.None;
	public bool reverse = false;
	public bool toggle = true;
	
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
