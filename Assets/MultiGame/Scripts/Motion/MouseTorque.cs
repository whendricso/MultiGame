using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class MouseTorque : MonoBehaviour {

	public float power = 10.0f;
	public bool maverickStyle = true;
	public KeyCode rotateRight = KeyCode.E;
	public KeyCode rotateLeft = KeyCode.Q;

	void FixedUpdate () {
		if (!maverickStyle) {
			if (Input.GetKey(rotateRight))
				rigidbody.AddRelativeTorque(-Vector3.forward * power);
			if (Input.GetKey(rotateLeft))
				rigidbody.AddRelativeTorque(Vector3.forward * power);
			rigidbody.AddRelativeTorque(new Vector3(-Input.GetAxis( "Mouse Y") * power, Input.GetAxis("Mouse X") * power, 0.0f), ForceMode.Force);
		}
		else {
			if (Input.GetKey(rotateRight))
				rigidbody.AddRelativeTorque(Vector3.up * power);
			if (Input.GetKey(rotateLeft))
				rigidbody.AddRelativeTorque(-Vector3.up * power);
			rigidbody.AddRelativeTorque(new Vector3(-Input.GetAxis( "Mouse Y") * power, 0.0f, -Input.GetAxis("Mouse X") * power), ForceMode.Force);
		}
	}
}
