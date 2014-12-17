using UnityEngine;
using System.Collections;

public class SpinMotor : MonoBehaviour {
	
	/// <summary>
	/// If no rigidbody, the number of degrees per second. Otherwise, the amount of relative torque per second.
	/// </summary>
	public Vector3 impetus = Vector3.zero;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GetComponent<Rigidbody>() == null) {
			transform.Rotate(impetus * Time.deltaTime);
		} else {
			rigidbody.AddRelativeTorque(impetus);
		}
	}
}
