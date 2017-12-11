using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class DoorController : MultiModule {

		public enum DoorTypes {Slide, Hinge};
		public DoorTypes doorType = DoorTypes.Slide;

		[Tooltip("How long does it take for the door to move?")]
		public float useTime = .8f;
		[Tooltip("What is it's hinge angle in open position? (Ignore for sliding doors)")]
		public float hingeAngle = 73f;
		[Tooltip("A curve representing the motion of the door,")]
		public AnimationCurve openCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
		[Tooltip("A curve representing the motion of the door,")]
		public AnimationCurve closeCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0f, 1f), new Keyframe(1f, 0f) });
		[Tooltip("A vector indicating the slide target along the door plane. (Ignore for hinged doors)")]
		public Vector2 slideVector = Vector2.right;

		private float startTime = 0f;
		public enum DoorStates {Open, Opening, Closed, Closing };
		[Tooltip("The default state of this door")]
		public DoorStates doorState = DoorStates.Closed;

		public HelpInfo help = new HelpInfo("Door Controller allows for hinged or sliding doors to be animated without creating new animation data. To use, parent a door model to the object that this component is attached to. " +
			"Move the parented door so that this object is at the hinge point of the door (for example, a revolving door should be centered on this object while a swinging door should be off to one side). Then, send messages to this " +
			"component from any Message sender, such as ClickMessage or ActiveZone when you want the door to open/close. The simplest message to send is 'ToggleDoor' but more can be found below.");

		void Start () {
			if (doorState == DoorStates.Opening)
				doorState = DoorStates.Open;
			if (doorState == DoorStates.Closing)
				doorState = DoorStates.Closed;
			startTime = Time.time;
		}

		void FixedUpdate () {
			if (doorType == DoorTypes.Slide) {
				if (doorState == DoorStates.Closing) {
					if ((Time.time - startTime) < useTime)
						transform.localPosition = new Vector3(slideVector.x * closeCurve.Evaluate((Time.time - startTime)/useTime), slideVector.y * closeCurve.Evaluate((Time.time - startTime)/useTime), 0f);
					else {
						transform.localPosition = Vector3.zero;
						doorState = DoorStates.Closed;
					}
				}
				if (doorState == DoorStates.Opening) {
					if ((Time.time - startTime) < useTime)
						transform.localPosition = new Vector3(slideVector.x * openCurve.Evaluate((Time.time - startTime)/useTime), slideVector.y * openCurve.Evaluate((Time.time - startTime)/useTime), 0f);
					else {
						transform.localPosition = new Vector3(slideVector.x, slideVector.y, 0f);
						doorState = DoorStates.Open;
					}
				}
			}

			if (doorType == DoorTypes.Hinge) {
				if (doorState == DoorStates.Closing) {
					if ((Time.time - startTime) < useTime)
						transform.localEulerAngles = new Vector3(0f, hingeAngle * closeCurve.Evaluate((Time.time - startTime)/useTime), 0f);
					else {
						transform.localRotation = Quaternion.identity;
						doorState = DoorStates.Closed;
					}
				}
				if (doorState == DoorStates.Opening) {
					if ((Time.time - startTime) < useTime)
						transform.localEulerAngles = new Vector3(0f, hingeAngle * openCurve.Evaluate((Time.time - startTime)/useTime), 0f);
					else {
						transform.localEulerAngles = new Vector3(0f, hingeAngle, 0f);
						doorState = DoorStates.Open;
					}
				}
			}
		}

		[Header("Available Messages")]
		public MessageHelp openDoorHelp = new MessageHelp("OpenDoor","Opens this door based on your settings");
		public void OpenDoor () {
			if (doorState != DoorStates.Closed)
				return;
			startTime = Time.time;
			doorState = DoorStates.Opening;
		}

		public MessageHelp closeDoorHelp = new MessageHelp("CloseDoor","Closes this door based on your settings");
		public void CloseDoor () {
			if (doorState != DoorStates.Open)
				return;
			startTime = Time.time;
			doorState = DoorStates.Closing;
		}

		public MessageHelp toggleDoorHelp = new MessageHelp("ToggleDoor","Opens or closes this door based on your settings");
		public void ToggleDoor () {
			if (doorState == DoorStates.Open) {
				startTime = Time.time;
				doorState = DoorStates.Closing;
			}
			if (doorState == DoorStates.Closed) {
				startTime = Time.time;
				doorState = DoorStates.Opening;
			}
		}
	}
}