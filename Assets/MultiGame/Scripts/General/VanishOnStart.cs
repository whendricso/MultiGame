using UnityEngine;
using System.Collections;

public class VanishOnStart : MonoBehaviour {

	void Start () {
		if(GetComponent<Renderer>() != null)
			GetComponent<Renderer>().enabled = false;
		else {
			gameObject.SetActive(false);
		}
	}
}
