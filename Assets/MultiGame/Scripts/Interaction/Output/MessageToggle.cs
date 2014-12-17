using UnityEngine;
using System.Collections;

public class MessageToggle : MonoBehaviour {

	public GameObject[] gameObjectTargets;
	public MonoBehaviour[] scriptTargets;
	public Light lightTarget;
	public bool invert = false;

	private bool previousVal;
	public bool debug = false;

	// Use this for initialization
	void Start () {
		if (invert)
			ToggleOff();
		if (gameObjectTargets.Length > 0)
			previousVal = !gameObjectTargets[0].activeSelf;
		else {
			previousVal = false;
			gameObjectTargets[0] = gameObject;
		}
		if (invert)
			previousVal = !previousVal;
	}

	void ToggleOn () {
		Toggle(true);
	}

	void ToggleOff () {
		Toggle(false);
	}

	void SwapToggle () {
		Toggle(previousVal);
	}

	void Toggle(bool val) {
		if (debug)
			Debug.Log("Toggle " + val);
		previousVal = !val;
		if (lightTarget != null)
			lightTarget.enabled = val;
		if (gameObjectTargets.Length > 0) {
			foreach (GameObject target in gameObjectTargets)
				target.SetActive(val);
		}
		if (scriptTargets.Length > 0) {
			foreach (MonoBehaviour target in scriptTargets) 
				target.enabled = val;
			
		}

	}

	void Activate () {
		SwapToggle();
	}
}
//Copyright 2014 William Hendrickson
