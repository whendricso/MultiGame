using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class DoorController : MultiModule {

		public enum DoorTypes {Slide, Hinge};
		public DoorTypes doorType = DoorTypes.Slide;

		public float useTime = .8f;
		public float hingeAngle = 73f;
		public AnimationCurve openCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
		public AnimationCurve closeCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0f, 1f), new Keyframe(1f, 0f) });
		public Vector2 slideVector = Vector2.right;

		private float startTime = 0f;
		public enum DoorStates {Open, Opening, Closed, Closing };
		public DoorStates doorState = DoorStates.Closed;

		public HelpInfo help = new HelpInfo("Door Controller allows for hinged or sliding doors to be animated without creating new animation data.");

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

		public void OpenDoor () {
			if (doorState != DoorStates.Closed)
				return;
			startTime = Time.time;
			doorState = DoorStates.Opening;
		}

		public void CloseDoor () {
			if (doorState != DoorStates.Open)
				return;
			startTime = Time.time;
			doorState = DoorStates.Closing;
		}

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