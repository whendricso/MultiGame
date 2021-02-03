using UnityEngine;
using MultiGame;

namespace MultiGame {
	public class SplineMotor : MultiModule {

		[RequiredFieldAttribute("A reference to the Bezier Spline component we wish to follow.")]
		public BezierSpline spline;
		[RequiredFieldAttribute("How long does it take for us to reach the end of the spline?")]
		public float duration = 1f;
		[Tooltip("Should we be in-motion by default?")]
		public bool running = true;
		[Tooltip("Should the object face the direction of motion automatically?")]
		public bool lookForward = true;
		public bool destroyOnSplineLoss = true;

		[Tooltip("Should we move along the spline only once, or should we keep going somehow?")]
		public SplineMotorMode mode = SplineMotorMode.Once;

		public MessageManager.ManagedMessage endOfPathMessage;

		private float progress;
		private bool goingForward = true;
		private bool endMessageSent = false;

		public HelpInfo help = new HelpInfo("Spline Motor allows an object to move along a given spline, assigned in the inspector. Select the Spline object to begin editing it.\n" +
			"\n" +
			"To add more complexity to the motion, select the Spline and click 'Add Node' in the Inspector. You can then click on a Node to move it, or select it's handles to change the curvature of the Spline. " +
			" To slow the object down, increase the 'Duration'. To create a looping or ping-pong motion, set the 'Mode'.", "https://youtu.be/uHvZw2q27H0");

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref endOfPathMessage, gameObject);
		}

		void Awake () {
			duration = Mathf.Abs(duration);
			if (endOfPathMessage.target == null)
				endOfPathMessage.target = gameObject;
			if (spline == null) {
				Debug.LogError("Spline Motor " + gameObject.name + " requires a Spline to be assigned in the inspector so that it has a path to follow!");
				enabled = false;
				return;
			}
		}

		void Start () {
			transform.SetParent(null);
		}

		private void Update () {
			if (destroyOnSplineLoss && spline == null)
				Destroy(gameObject);
			if (spline == null) {
				MessageManager.Send(endOfPathMessage);
				return;
			}
			if (!running)
				return;
			duration = Mathf.Abs (duration);
			if (goingForward) {
				progress += Time.deltaTime / duration;
				if (progress > 1f) {
					if (mode == SplineMotorMode.Once) {
						progress = 1f;
						if (!endMessageSent) {
							endMessageSent = true;
							MessageManager.Send(endOfPathMessage);
						}
					}
					else if (mode == SplineMotorMode.Loop) {
						progress -= 1f;
						MessageManager.Send(endOfPathMessage);
					}
					else {
						progress = 2f - progress;
						goingForward = false;
						MessageManager.Send(endOfPathMessage);
					}
				}
			}
			else {
				progress -= Time.deltaTime / duration;
				if (progress < 0f) {
					progress = -progress;
					goingForward = true;
				}
			}

			Vector3 position = spline.GetPoint(progress);
			transform.localPosition = position;
			if (lookForward) {
				transform.LookAt(position + spline.GetDirection(progress));
				
			}
		}

		public MessageHelp resetMotionHelp = new MessageHelp("ResetMotion","Starts the motor over from the beginning");
		public void ResetMotion () {
			progress = 0f;
		}

		public MessageHelp setProgressHelp = new MessageHelp("SetProgress","Allows you to place the motor anywhere along the spline",3,"A percentage of progress between 0 and 1");
		public void SetProgress(float _progress) {
			progress = Mathf.Clamp01(_progress);
		}

		public MessageHelp goForwardHelp = new MessageHelp("GoForward","Forces the motor to move in a forward direction along the spline.");
		public void GoForward () {
			goingForward = true;
		}

		public MessageHelp goBackwardHelp = new MessageHelp("GoBackward","Forces the motor to move in a backward direction along the spline.");
		public void GoBackward () {
			goingForward = false;
		}

		public MessageHelp reverseDirectionHelp = new MessageHelp("ReverseDirection","Forces the motor to move in the opposite direction from it's current heading along the spline.");
		public void ReverseDirection () {
			goingForward = !goingForward;
		}

		public MessageHelp setDurationHelp = new MessageHelp("SetDuration","Allows you to change the total travel time for the motor",3,"The new duration, the time it takes for the motor to reach the end of the spline.");
		public void SetDuration (float _duration) {
			duration = _duration;
		}

		public MessageHelp stopHelp = new MessageHelp("Stop","Stop moving immediately, and stay at this position");
		public void Stop () {
			running = false;
		}

		public MessageHelp resumeHelp = new MessageHelp("Resume","Continue moving along the spline if stopped");
		public void Resume () {
			running = true;
		}

		public MessageHelp toggleHelp = new MessageHelp("Toggle","If moving, stop here. Otherwise, continue moving.");
		public void Toggle () {
			running = !running;
		}
	}
}