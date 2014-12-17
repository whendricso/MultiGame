using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Light))]
public class Flashlight : MonoBehaviour {
	
	public KeyCode flashlightToggle = KeyCode.F;
	
	void Start () {
		light.enabled = false;
	}
	
	void Update () {
		if (Input.GetKeyDown(flashlightToggle))
			light.enabled = !light.enabled;
	}
}
