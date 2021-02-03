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
		public Space space = Space.Self;
		public enum UpdateModes {Update, FixedUpdate, LateUpdate };
		public UpdateModes updateMode = UpdateModes.FixedUpdate;

		private Rigidbody rigid;

		public HelpInfo help = new HelpInfo("This component, similar to ConstantForce component, adds a constant spin or torque instead. Rigidbody use is optional. To use, add to any object that you would like to spin, and " +
			"either set the rotation speed/force above in the 'Impetus' setting, or send messages to set the spin at runtime.");

		void Start () {
			rigid = GetComponent<Rigidbody> ();
		}

		private void Update() {
			if (updateMode == UpdateModes.Update)
				UpdateRotation();
		}

		void FixedUpdate () {
			if (updateMode == UpdateModes.FixedUpdate)
				UpdateRotation();
		}

		private void LateUpdate() {
			if (updateMode == UpdateModes.LateUpdate)
				UpdateRotation();
		}

		void UpdateRotation() {
			if (rigid == null) {
				transform.Rotate(impetus * Time.deltaTime, space);
			} else {
				if (space == Space.Self)
					rigid.AddRelativeTorque(impetus);
				else
					rigid.AddTorque(impetus);
			}
		}

		[Header("Available Messages")]
		public MessageHelp setImpetusXHelp = new MessageHelp("SpinImpetusX", "Changes the impetus on the X axis.",3,"The new rotational impetus");
		public void SpinImpetusX(float _impetus) {
			impetus.x = _impetus;
		}

		public MessageHelp setImpetusYHelp = new MessageHelp("SpinImpetusY", "Changes the impetus on the Y axis.",3,"The new rotational impetus");
		public void SpinImpetusY(float _impetus) {
			impetus.y = _impetus;
		}
		public MessageHelp setImpetusZHelp = new MessageHelp("SpinImpetusZ", "Changes the impetus on the Z axis.",3,"The new rotational impetus");
		public void SpinImpetusZ(float _impetus) {
			impetus.z = _impetus;
		}
		public MessageHelp rotateXByHelp = new MessageHelp("RotateXBy", "Rotates the object on the X axis once",3,"How much should we rotate?");
		public void RotateXBy(float _impetus) {
			transform.Rotate(space == Space.Self ? transform.right : Vector3.right, _impetus);
		}
		public MessageHelp rotateYByHelp = new MessageHelp("RotateYBy", "Rotates the object on the Y axis once",3,"How much should we rotate?");
		public void RotateYBy(float _impetus) {
			transform.Rotate(space == Space.Self ? transform.up : Vector3.up, _impetus);
		}
		public MessageHelp rotateZByHelp = new MessageHelp("RotateZBy", "Rotates the object on the Z axis once",3,"How much should we rotate?");
		public void RotateZBy(float _impetus) {
			transform.Rotate(space == Space.Self ? transform.forward : Vector3.forward, _impetus);
		}
	}
}