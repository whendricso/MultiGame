using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent (typeof (Rigidbody))]
	public class MouseTorque : MultiModule {

		[Tooltip("How strong is our input?")]
		public float power = 10.0f;
		[Tooltip("Are you the Ice Man?")]
		public bool maverickStyle = true;
		public KeyCode rotateRight = KeyCode.E;
		public KeyCode rotateLeft = KeyCode.Q;

		public HelpInfo help = new HelpInfo("This component applies torque based on mouse movement");

		void FixedUpdate () {
			if (!maverickStyle) {
				if (Input.GetKey(rotateRight))
					GetComponent<Rigidbody>().AddRelativeTorque(-Vector3.forward * power);
				if (Input.GetKey(rotateLeft))
					GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * power);
				GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-Input.GetAxis( "Mouse Y") * power, Input.GetAxis("Mouse X") * power, 0.0f), ForceMode.Force);
			}
			else {
				if (Input.GetKey(rotateRight))
					GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * power);
				if (Input.GetKey(rotateLeft))
					GetComponent<Rigidbody>().AddRelativeTorque(-Vector3.up * power);
				GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-Input.GetAxis( "Mouse Y") * power, 0.0f, -Input.GetAxis("Mouse X") * power), ForceMode.Force);
			}
		}
	}
}