using UnityEngine;
using System.Collections;

public class MessageToggle : MultiModule {

	[Tooltip("Game Objects we will toggle")]
	public GameObject[] gameObjectTargets;
	[Tooltip("Scripts we will toggle")]
	public MonoBehaviour[] scriptTargets;
	[Tooltip("Light we wish to toggle")]
	public Light lightTarget;
	[Tooltip("Should we toggle the opposite way?")]
	public bool invert = false;

	private bool previousVal;

	public HelpInfo help = new HelpInfo("This component toggles scripts and game objects based on messages. The 'Toggle' message takes a boolean value");

	public bool debug = false;

	// Use this for initialization
	void Start () {
		if (invert)
			ToggleOff();
		if (gameObjectTargets.Length > 0)
			previousVal = !gameObjectTargets[0].activeSelf;
		else {
			previousVal = false;
//			gameObjectTargets[0] = gameObject;
		}
		if (invert)
			previousVal = !previousVal;
	}

	public void ToggleOn () {
		Toggle(true);
	}

	public void ToggleOff () {
		Toggle(false);
	}

	public void SwapToggle () {
		Toggle(previousVal);
	}

	public void Toggle(bool val) {
		if (debug)
			Debug.Log("Toggle " + val);
		previousVal = !val;
		if (lightTarget != null)
			lightTarget.enabled = val;
		if (gameObjectTargets.Length > 0) {
			foreach (GameObject target in gameObjectTargets)
				if (target != null)
					target.SetActive(val);
		}
		if (scriptTargets.Length > 0) {
			foreach (MonoBehaviour target in scriptTargets) {
				if (target != null)
					target.enabled = val;
			}
			
		}

	}
}
//MultiGame Copyright 2014 William Hendrickson
