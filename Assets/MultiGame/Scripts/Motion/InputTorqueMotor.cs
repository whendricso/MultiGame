using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Input Torque")]
	[RequireComponent(typeof(Rigidbody))]
	public class InputTorqueMotor : MultiModule {

		public string axis = "Vertical";
		public float power = 10.0f;
		public Vector3 outputAxes = Vector3.right;

		private bool constantTorque = false;

		public HelpInfo help = new HelpInfo("This component applies Rigidbody torque based on the horizontal and vertical axes. To use, put the top torque in for the X and Y axes. Z is ignored. These automatically " +
			"rotate on the X axis (Vertical) and Y axis (horizontal) rotations.");

		private Rigidbody rigid;

		void OnEnable () {
			if (rigid == null)
				rigid = GetComponent<Rigidbody> ();
		}

		void FixedUpdate () {
			if (constantTorque)
				rigid.AddRelativeTorque(((power) * outputAxes));
			if (!string.IsNullOrEmpty(axis))
				rigid.AddRelativeTorque(((power * Input.GetAxis(axis)) * outputAxes));
		}

		public MessageHelp activateTorqueHelp = new MessageHelp("ActivateTorque","Starts the torque motor");
		public void ActivateTorque() {
			constantTorque = true;
		}

		public MessageHelp deactivateTorqueHelp = new MessageHelp("DeactivateTorque","Stops the torque motor");
		public void DeactivateTorque() {
			constantTorque = false;
		}
	}
}