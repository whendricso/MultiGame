using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class FighterInputController : MonoBehaviour {
	
	public Vector3 thrust = new Vector3(40.0f, 180.0f, 400.0f);
	public float constantThrust = 0.0f;
	private float axisX = 0.0f;
	private float axisY = 0.0f;
	private float axisZ = 0.0f;
	public KeyCode upThrusters = KeyCode.Space;
	public KeyCode downThrusters = KeyCode.LeftShift;
	public float pitchTorque = 60.0f;
	public float yawTorque = 60.0f;
	public float rudderTorque = 240.0f;
	public KeyCode pitchCCW = KeyCode.Q;
	public KeyCode pitchCW = KeyCode.E;
	public KeyCode levelOut = KeyCode.LeftControl;
	private bool levelingOut = false;
	
	void FixedUpdate () {
		if (constantThrust > 0)
			GetComponent<Rigidbody>().AddRelativeForce(0.0f, 0.0f, constantThrust, ForceMode.Force);
		ProcessInputThrust();
	}
	
	public void ProcessInputThrust() {
		axisX = thrust.x * Input.GetAxis("Horizontal");
		axisZ = thrust.z * Input.GetAxis("Vertical");
		if (!Input.GetKey(upThrusters) && !Input.GetKey(downThrusters))
			axisY = 0.0f;
		if(Input.GetKey(downThrusters))
			axisY = -thrust.y;
		if(Input.GetKey(upThrusters))
			axisY = thrust.y;
		
		else
		
		GetComponent<Rigidbody>().AddRelativeForce(axisX, axisY, axisZ, ForceMode.Force);
		GetComponent<Rigidbody>().AddRelativeTorque(Input.GetAxis("Mouse Y") * yawTorque * -1, Input.GetAxis("Mouse X") * rudderTorque, 0.0f);
		
		if (Input.GetKey(pitchCW))
			GetComponent<Rigidbody>().AddRelativeTorque(0.0f, 0.0f, -pitchTorque);
		if (Input.GetKey(pitchCCW))
			GetComponent<Rigidbody>().AddRelativeTorque(0.0f, 0.0f, pitchTorque);
		

	}
	

}
