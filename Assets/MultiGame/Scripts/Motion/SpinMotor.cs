using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Spin Motor")]
	public class SpinMotor : MultiModule {
		
		/// <summary>
		/// If no rigidbody, the number of degrees per second. Otherwise, the amount of relative torque per second.
		/// </summary>
		[Tooltip("If no rigidbody, the number of degrees per second per axis, otherwise the amount of torque per fixed update")]
		public Vector3 impetus = Vector3.zero;

		private Rigidbody rigid;

		public HelpInfo help = new HelpInfo("This component, similar to ConstantForce component, adds a constant spin or torque instead. Rigidbody use is optional. To use, add to any object that you would like to spin, and " +
			"either set the rotation speed/force above in the 'Impetus' setting, or send messages to set the spin at runtime.");

		void Awake () {
			rigid = GetComponent<Rigidbody> ();
		}

		// Update is called once per frame
		void FixedUpdate () {
			if (rigid == null) {
				transform.Rotate(impetus * Time.deltaTime);
			} else {
				rigid.AddRelativeTorque(impetus);
			}
		}

		[Header("Available Messages")]
		public MessageHelp setImpetusXHelp = new MessageHelp("SetImpetusX","Changes the impetus on the X axis.",3,"The new rotational impetus");
		public void SetImpetusX(float _impetus) {
			impetus.x = _impetus;
		}

		public MessageHelp setImpetusYHelp = new MessageHelp("SetImpetusY","Changes the impetus on the Y axis.",3,"The new rotational impetus");
		public void SetImpetusY(float _impetus) {
			impetus.y = _impetus;
		}
		public MessageHelp setImpetusZHelp = new MessageHelp("SetImpetusZ","Changes the impetus on the Z axis.",3,"The new rotational impetus");
		public void SetImpetusZ(float _impetus) {
			impetus.z = _impetus;
		}
	}
}