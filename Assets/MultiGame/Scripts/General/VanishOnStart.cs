using UnityEngine;
using System.Collections;

public class VanishOnStart : MonoBehaviour {

	void Start () {
		if(renderer != null)
			renderer.enabled = false;
		else {
			gameObject.SetActive(false);
		}
	}
}
