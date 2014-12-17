using UnityEngine;
using System.Collections;

public class InputRotator : MonoBehaviour {

	public Vector3 impetus;
	
	void FixedUpdate () {
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.02) {
			transform.RotateAround(transform.position, Vector3.up, impetus.y * Input.GetAxis("Horizontal"));
		}
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.02) {
			transform.RotateAround(transform.position, Vector3.right, impetus.x * Input.GetAxis("Vertical"));
		}
		
	}
}
//Copyright 2014 William Hendrickson
