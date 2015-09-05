using UnityEngine;
using System.Collections;

public class SpinMotor : MultiModule {
	
	/// <summary>
	/// If no rigidbody, the number of degrees per second. Otherwise, the amount of relative torque per second.
	/// </summary>
	[Tooltip("If no rigidbody, the number of degrees per second per axis, otherwise the amount of torque per fixed update")]
	public Vector3 impetus = Vector3.zero;

	public HelpInfo help = new HelpInfo("This component, similar to ConstantForce component, adds a constant spin or torque instead. Rigidbody use is optional.");
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GetComponent<Rigidbody>() == null) {
			transform.Rotate(impetus * Time.deltaTime);
		} else {
			GetComponent<Rigidbody>().AddRelativeTorque(impetus);
		}
	}
}
