using UnityEngine;
using System.Collections;
//Un-parents the object as soon as it's born.
//Useful for making prefabs out of multiple rigidbodies, and other things :)


public class DetatchOnStart : MonoBehaviour {

	void Start () {
		transform.parent = null;
	}
}
