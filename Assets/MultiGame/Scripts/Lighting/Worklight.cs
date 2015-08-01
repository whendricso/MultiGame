using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]
public class Worklight : MonoBehaviour {
	
	
	
	void Start () {
		GetComponent<Light>().enabled = false;
	}
}
