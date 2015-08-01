using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Light))]
public class Flashlight : MonoBehaviour {
	
	public KeyCode flashlightToggle = KeyCode.F;
	
	void Start () {
		GetComponent<Light>().enabled = false;
	}
	
	void Update () {
		if (Input.GetKeyDown(flashlightToggle))
			GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
	}
}
