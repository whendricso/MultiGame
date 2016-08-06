using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Six Axis")]
	[RequireComponent (typeof(Rigidbody))]
	public class SixAxis : MultiModule {

		public float forwardThrust = 10.0f;
		public float sidewaysThrust = 10.0f;
		public float reverseThrust = 10.0f;
		public float upwardThrust = 10.0f;
		public float downwardThrust = 10.0f;
		[RequiredFieldAttribute("How much dead space is in the center of the control stick")]
		public float deadzone = 0.25f;
		public KeyCode upKey = KeyCode.Space;
		public KeyCode downKey = KeyCode.LeftShift;
		[Tooltip("Should we update input after the main loop instead of during?")]
		public bool useLateUpdate = true;
		private Vector2 stickInput = Vector2.zero;
		private bool goUp = false;
		private bool goDown = false;
		private Vector3 thrustVec = Vector3.zero;

		[Tooltip("The rigidbody we will be applying force to")]
		public Rigidbody body;

		public HelpInfo help = new HelpInfo("This component is a player input controller allowing the user to fly in all directions, truly utilizing 3D space. It takes input from the " +
			"user, processes it for precision, and applies it as force to the attached Rigidbody");

		void Start () {
			if (body == null)
				body = GetComponent<Rigidbody>();
			if (body == null) {
				Debug.LogError("Six Axis " + gameObject.name + " needs a rigidbody assigned in the inspector or attached to the object to function");
			}
		}

		void Update () {
			if (!useLateUpdate)
				UpdateInputState();
		}

		void LateUpdate () {
			if (useLateUpdate)
				UpdateInputState();
		}

		void UpdateInputState () {
			stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			if(stickInput.magnitude < deadzone)
				stickInput = Vector2.zero;
			else
				stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));
			
			if (Input.GetKeyDown(upKey))
				goUp = true;
			if (Input.GetKeyUp(upKey))
				goUp = false;
			if (Input.GetKeyDown(downKey))
				goDown = true;
			if (Input.GetKeyUp(downKey))
				goDown = false;
		}
		
		void FixedUpdate () {

			if (stickInput.y > 0.0f)
				thrustVec.z = forwardThrust * stickInput.y;
			if (stickInput.y < 0.0f)
				thrustVec.z = reverseThrust * stickInput.y;
			if (stickInput.y == 0.0f)
				thrustVec.z = 0.0f;
			if (goUp)
				thrustVec.y = upwardThrust;
			if (goDown)
				thrustVec.y = -downwardThrust;
			if (!goUp && !goDown)
				thrustVec.y = 0.0f;
			thrustVec.x = stickInput.x * sidewaysThrust;

			body.AddRelativeForce(thrustVec);
			
		}
	}
}