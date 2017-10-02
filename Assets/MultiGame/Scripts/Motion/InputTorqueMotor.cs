using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Input Torque")]
	[RequireComponent(typeof(Rigidbody))]
	public class InputTorqueMotor : MultiModule {

		public string axis = "Vertical";
		public float power = 10.0f;
		public Vector2 outputAxes = Vector2.right;

		public HelpInfo help = new HelpInfo("This component applies Rigidbody torque based on the horizontal and vertical axes. To use, put the top torque in for the X and Y axes. Z is ignored. These automatically " +
			"rotate on the X axis (Vertical) and Y axis (horizontal) rotations.");

		private Rigidbody rigid;

		void Start () {
			rigid = GetComponent<Rigidbody> ();
		}

		void FixedUpdate () {
			rigid.AddRelativeTorque(((power * Input.GetAxis(axis)) * outputAxes)*Time.deltaTime);
		}
	}
}