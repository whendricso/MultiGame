using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HingeJoint))]
public class HingeMotor : MonoBehaviour {

	public KeyCode forward = KeyCode.W;
	public KeyCode reverse = KeyCode.S;

	public float force = 10.0f;

	[HideInInspector]
	public HingeJoint joint;

	void Start () {
		joint = GetComponent<HingeJoint>();
	}
	
	void FixedUpdate () {
		JointMotor motor = new JointMotor();
		motor = joint.motor;

		if (Input.GetKey(forward))
			motor.force = force;
		if (Input.GetKey (reverse))
			motor.force = -force;

		if (!Input.GetKey(forward) && !Input.GetKey(reverse))
			motor.force = 0f;

		joint.motor = motor;
	}
}
