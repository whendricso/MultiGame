using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Mouse Torque")]
	[RequireComponent (typeof (Rigidbody))]
	public class MouseTorque : MultiModule {

		[Tooltip("How strong is our input to torque ratio?")]
		public float power = 10.0f;
		[Tooltip("Are you the Ice Man? (Reverses the vertical axis)")]
		public bool maverickStyle = true;
		[Tooltip("Optional key to rotate around the Z axis")]
		public KeyCode rotateRight = KeyCode.E;
		[Tooltip("Optional key to rotate around the Z axis")]
		public KeyCode rotateLeft = KeyCode.Q;

		public HelpInfo help = new HelpInfo("This component applies torque based on mouse movement");

		void FixedUpdate () {
			if (!maverickStyle) {
				if (Input.GetKey(rotateRight))
					GetComponent<Rigidbody>().AddRelativeTorque((-Vector3.forward * power) * Time.deltaTime);
				if (Input.GetKey(rotateLeft))
					GetComponent<Rigidbody>().AddRelativeTorque((Vector3.forward * power) * Time.deltaTime);
				GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-Input.GetAxis( "Mouse Y") * power, Input.GetAxis("Mouse X") * power, 0.0f) * Time.deltaTime, ForceMode.Force);
			}
			else {
				if (Input.GetKey(rotateRight))
					GetComponent<Rigidbody>().AddRelativeTorque((Vector3.up * power)*Time.deltaTime);
				if (Input.GetKey(rotateLeft))
					GetComponent<Rigidbody>().AddRelativeTorque((-Vector3.up * power) * Time.deltaTime);
				GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(-Input.GetAxis( "Mouse Y") * power, 0.0f, -Input.GetAxis("Mouse X") * power) * Time.deltaTime, ForceMode.Force);
			}
		}
	}
}