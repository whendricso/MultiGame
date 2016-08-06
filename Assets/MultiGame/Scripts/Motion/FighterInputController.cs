using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Fighter Input Controller")]
	[RequireComponent (typeof(Rigidbody))]
	public class FighterInputController : MultiModule {
		
		[Tooltip("How much thrust do the engines have in each direction?")]
		public Vector4 thrust = new Vector4(40.0f, 180.0f, 400.0f, 120f);
		[Tooltip("How much constant forward thrust should be applied?")]
		public float constantThrust = 0.0f;
		private float axisX = 0.0f;
		private float axisY = 0.0f;
		private float axisZ = 0.0f;
		[Tooltip("Key for activating elevators")]
		public KeyCode upThrusters = KeyCode.Space;
		[Tooltip("Key for activating down thrusters")]
		public KeyCode downThrusters = KeyCode.LeftShift;
		[Tooltip("How hard should we pitch?")]
		public float pitchTorque = 60.0f;
		[Tooltip("How hard should we yaw?")]
		public float yawTorque = 60.0f;
		[Tooltip("How much force is applied by the rudder?")]
		public float rudderTorque = 240.0f;
		[Tooltip("Key for pitching counter-clockwise")]
		public KeyCode pitchCCW = KeyCode.Q;
		[Tooltip("Key for pitching clockwise")]
		public KeyCode pitchCW = KeyCode.E;
	//	public KeyCode levelOut = KeyCode.LeftControl;
	//	private bool levelingOut = false;

		public HelpInfo help = new HelpInfo("This component implements physics-based flight control with motion similar to that of a fighter jet (or starfighter). 'Thrust' indicates " +
			"how much power the engines have in each axis, except that W represents the negative Z axis (so that you can have a different speed in reverse).");

		public Rigidbody body;

		void Start () {
			body = GetComponent<Rigidbody>();
		}

		void FixedUpdate () {
			if (constantThrust > 0)
				GetComponent<Rigidbody>().AddRelativeForce(0.0f, 0.0f, constantThrust, ForceMode.Force);
			ProcessInputThrust();
		}
		
		public void ProcessInputThrust() {
			axisX = thrust.x * Input.GetAxis("Horizontal");
			if (Input.GetAxis("Vertical") >= 0)
				axisZ = thrust.z * Input.GetAxis("Vertical");
			else
				axisZ = (-thrust.w) * Input.GetAxis("Vertical");
			
			if(Input.GetKey(downThrusters))
				axisY = -thrust.y;
			if(Input.GetKey(upThrusters))
				axisY = thrust.y;
			if (!Input.GetKey(upThrusters) && !Input.GetKey(downThrusters))
				axisY = 0.0f;
			
			else
			
				body.AddRelativeForce(axisX, axisY, axisZ, ForceMode.Force);
				body.AddRelativeTorque(Input.GetAxis("Mouse Y") * yawTorque * -1, Input.GetAxis("Mouse X") * rudderTorque, 0.0f);
			
			if (Input.GetKey(pitchCW))
				body.AddRelativeTorque(0.0f, 0.0f, -pitchTorque);
			if (Input.GetKey(pitchCCW))
				body.AddRelativeTorque(0.0f, 0.0f, pitchTorque);
			

		}
		

	}
}