using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Thruster")]
	public class Thruster : MultiModule {

		[Tooltip("Are we currently thrusting?")]
		public bool thrusting = false;
		[Tooltip("How much thrust do we apply each fixed update?")]
		public Vector3 thrust;
		[Tooltip("If we are using an input axis, which one?")]
		public string axis = "Vertical";
		[Tooltip("How should the force be applied?")]
		public Space space = Space.Self;
		[Tooltip("Optional target to transfer force to")]
		public GameObject target;

		[Tooltip("Should we use an input axis to control the thrust level?")]
		public bool useInputAxis = false;
		[Tooltip("How sensitive is that axis?")]
		public float inputSensitivity = 0.2f;

		private Rigidbody rigid;
		private bool useTargetRigidbody = false;

		public HelpInfo help = new HelpInfo("This component adds thrust to a given Rigidbody. It also works with the 'InputVector' component (optionally) allthoug it can be used" +
			" either by itself or with any message sender/toggle component." +
			"\n---Messages:---\n" +
			"'BeginThrust' and 'EndThrust' send the predetermined amount of force to the rigidbody each frame until stopped.\n" +
			"'ThrustAmount' takes a float representing how much to multiply the 'Thrust' vector and add it once this frame.");

		void Start () {
			if (GetComponent<Rigidbody>() == null && target == null) {
				Debug.LogError("Thruster " + gameObject.name + "must have attached rigidbody or a target with a rigidbody to work!");
				enabled = false;
				return;
			}
			if (target.GetComponent<Rigidbody>() == null) {
				Debug.LogError("Thruster " + gameObject.name + "must have attached rigidbody or a target with a rigidbody to work!");
				enabled = false;
				return;
			}
			if (target != null) {
				useTargetRigidbody = true;
				rigid = target.GetComponent<Rigidbody>();
			}
		}

		void FixedUpdate () {
			if (!useInputAxis) {
				if (thrusting) {
					if (space == Space.Self) {
						if (!useTargetRigidbody)
							GetComponent<Rigidbody>().AddRelativeForce(thrust, ForceMode.Force);
						else
							rigid.AddRelativeForce(thrust, ForceMode.Force);
							

					}
					else {
						if (!useTargetRigidbody)
							GetComponent<Rigidbody>().AddForce(thrust, ForceMode.Force);
						else
							rigid.AddForce(thrust, ForceMode.Force);
							
					}
				}
			}
			else {//use input axis
				if ( Mathf.Abs(Input.GetAxis(axis)) > inputSensitivity) {
					if (space == Space.Self) {
						if (!useTargetRigidbody)
							GetComponent<Rigidbody>().AddRelativeForce(thrust * Input.GetAxis(axis), ForceMode.Force);
						else
							rigid.AddRelativeForce(thrust * Input.GetAxis(axis), ForceMode.Force);
							
					}
					else {
						if (!useTargetRigidbody)
							GetComponent<Rigidbody>().AddForce(thrust * Input.GetAxis(axis), ForceMode.Force);
						else
							rigid.AddForce(thrust * Input.GetAxis(axis), ForceMode.Force);
							
					}
				}
			}
		}

		public void BeginThrust () {
			thrusting = true;
		}

		public void EndThrust () {
			thrusting = false;
		}

		public void ThrustAmount (float scalar) {
			if (scalar != 0.0f) {
				if (space == Space.Self)
					GetComponent<Rigidbody>().AddRelativeForce(thrust * scalar);
				else
					GetComponent<Rigidbody>().AddForce(thrust * scalar, ForceMode.Force);
			}
		}

		public void ThrustVector (Vector3 input) {
			Debug.Log("Thrust " + input);
			if (space == Space.Self)
				GetComponent<Rigidbody>().AddRelativeForce(new Vector3( input.x * thrust.x, input.y * thrust.y, input.z * thrust.z));
			else
				GetComponent<Rigidbody>().AddForce(new Vector3( input.x * thrust.x, input.y * thrust.y, input.z * thrust.z));
		}
	}
}
//Copyright 2014 William Hendrickson
