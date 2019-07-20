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
		[Tooltip("Should the force be applied in global or local coordinates?")]
		public Space space = Space.Self;
		[Tooltip("If true, the thruster will always apply force to the center of the rigidbody. Otherwise, it will apply force at it's position")]
		public bool ignoreThrusterPosition = true;
		[Tooltip("Optional target to transfer force to")]
		public GameObject target;

		[Tooltip("Should we use an input axis to control the thrust level? X and Y input axes correspond to X and Z thrust.")]
		public bool useInputAxis = false;
		[Tooltip("How sensitive is that axis?")]
		public float inputSensitivity = 0.2f;
		[Tooltip("If a Particle Controller component is present, the Thruster will control it's Emission multiplier as a percentage of current thrust")]
		public ParticleController controller;
		private Rigidbody rigid;
		//private bool useTargetRigidbody = false;

		public HelpInfo help = new HelpInfo("This component adds thrust to a given Rigidbody. It also works with the 'InputVector' component (optionally) allthoug it can be used" +
			" either by itself or with any message sender/toggle component. To use, add this to an object with a Rigidbody component that you'd like to push around. Then, input some 'Thrust' settings above to tell the Thruster " +
			"how strong it is in which directions. Negative values may be used. Finally, either set it's 'Thrusting' setting to true, or send messages to it to control it's thrust state.");

		void OnEnable () {
			if (controller == null)
				controller = GetComponent<ParticleController>();
			if (controller == null)
				controller = GetComponentInChildren<ParticleController>();
			if (target == null)
				target = gameObject;
			rigid = target.GetComponent<Rigidbody>();
			if (rigid == null) {
				Debug.LogError("Thruster " + gameObject.name + "must have attached rigidbody or a target with a rigidbody to work!");
				enabled = false;
				return;
			}
		}

		void FixedUpdate () {
			if (!useInputAxis) {
				if (thrusting) {
					if (space == Space.Self) {
						if (ignoreThrusterPosition)
							rigid.AddRelativeForce(thrust, ForceMode.Force);
						else
							rigid.AddForceAtPosition(transform.TransformVector(thrust), transform.position, ForceMode.Force);
					}
					else {
						if (ignoreThrusterPosition)
							rigid.AddForce(thrust, ForceMode.Force);
						else
							rigid.AddForceAtPosition(thrust, transform.position, ForceMode.Force);
							
					}
				}
			}
			else {//use input axis
				if ( Mathf.Abs(Input.GetAxis(axis)) > inputSensitivity) {
					if (space == Space.Self) {
						if (ignoreThrusterPosition)
							rigid.AddRelativeForce(thrust * Input.GetAxis(axis), ForceMode.Force);
						else
							rigid.AddForceAtPosition(transform.TransformVector(thrust) * Input.GetAxis(axis), transform.position, ForceMode.Force);

					}
					else {
						if (ignoreThrusterPosition)
							rigid.AddForce(thrust * Input.GetAxis(axis), ForceMode.Force);
						else
							rigid.AddForceAtPosition(thrust * Input.GetAxis(axis), transform.position, ForceMode.Force);

					}
				}
			}
		}

		[Header("Available Messages")]
		public MessageHelp beginThrustHelp = new MessageHelp("BeginThrust","Start to send the predetermined amount of force to the rigidbody each frame until stopped.");
		public void BeginThrust () {
			thrusting = true;
			if (controller != null)
				controller.FadeIn();
		}

		public MessageHelp endThrustHelp = new MessageHelp("EndThrust","Stop thrusting");
		public void EndThrust () {
			thrusting = false;
			if (controller != null)
				controller.FadeOut();
		}

		public MessageHelp thrustAmountHelp = new MessageHelp("ThrustAmount","Thrust a specific amount this frame",3,"The scalar of thrust we want to send (multiplied by the 'Thrust' you indicated above)");
		public void ThrustAmount (float scalar) {
			if (!gameObject.activeInHierarchy)
				return;
			if (scalar != 0.0f) {
				if (space == Space.Self) {
					if (ignoreThrusterPosition)
						rigid.AddRelativeForce(thrust * scalar, ForceMode.Force);
					else
						rigid.AddForceAtPosition(transform.TransformVector(thrust) * scalar,transform.position, ForceMode.Force);
				} else {
					if (ignoreThrusterPosition)
						rigid.AddForce(thrust * scalar, ForceMode.Force);
					else
						rigid.AddForceAtPosition(thrust * scalar, transform.position, ForceMode.Force);
				}
			}
		}

		public void ThrustVector (Vector3 input) {
			if (!gameObject.activeInHierarchy)
				return;
			if (space == Space.Self) {
				if (ignoreThrusterPosition)
					rigid.AddRelativeForce(new Vector3(input.x * thrust.x, input.y * thrust.y, input.z * thrust.z),ForceMode.Force);
				else
					rigid.AddForceAtPosition(transform.TransformVector( new Vector3(input.x * thrust.x, input.y * thrust.y, input.z * thrust.z)),transform.position, ForceMode.Force);
			} else {
				if (ignoreThrusterPosition)
					rigid.AddForce(new Vector3(input.x * thrust.x, input.y * thrust.y, input.z * thrust.z), ForceMode.Force);
				else
					rigid.AddForceAtPosition(new Vector3(input.x * thrust.x, input.y * thrust.y, input.z * thrust.z),transform.position, ForceMode.Force);
			}
			/*
			if (controller != null) {
				if (useInputAxis) {
					return;
				}
					int _axesCount = 0;
					float total = 0;
					if (Mathf.Abs(input.x) > 0) {
						_axesCount++;
						total += Mathf.Abs(input.x);
					}
					if (Mathf.Abs(input.y) > 0) {
						_axesCount++;
						total += Mathf.Abs(input.y);
					}
					if (Mathf.Abs(input.z) > 0) {
						_axesCount++;
						total += Mathf.Abs(input.z);
					}
					if (_axesCount > 0)
						controller.SetRatePercent(total / _axesCount);
					else
						controller.SetRatePercent(0);
				} else {
					if (thrusting)
						controller.SetRatePercent(1);
					else
						controller.SetRatePercent(0);
				//}
			}*/
		}
	}
}
//Copyright 2014 William Hendrickson
