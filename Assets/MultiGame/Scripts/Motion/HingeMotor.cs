using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/HingeMotor")]
	[RequireComponent(typeof(HingeJoint))]
	public class HingeMotor : MultiModule {

		public KeyCode forward = KeyCode.W;
		public KeyCode reverse = KeyCode.S;
		[Tooltip("How strong is this motor?")]
		public float force = 10.0f;

		[HideInInspector]
		public HingeJoint joint;

		public HelpInfo help = new HelpInfo("This component allows a hinge joint to be used as a motor. It takes input from the player and applies torque to the hinge.");

		void OnEnable () {
			if (joint == null)
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
}