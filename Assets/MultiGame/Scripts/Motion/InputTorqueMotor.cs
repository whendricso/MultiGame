using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class InputTorqueMotor : MultiModule {

	public string axis = "Vertical";
	public float power = 10.0f;
	public Vector3 outputAxes = Vector3.right;

	public HelpInfo help = new HelpInfo("This component applies Rigidbody torque based on the value of an input axis.");
	
	void FixedUpdate () {
		GetComponent<Rigidbody>().AddRelativeTorque((power * Input.GetAxis(axis)) * outputAxes);
	}
}
