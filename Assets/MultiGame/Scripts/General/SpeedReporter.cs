using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Speed Reporter")]
	public class SpeedReporter : MultiModule {

		public enum ReportTypes { Debug, GUI};
		[Tooltip("Should we show the speed in the console, or in a legacy Unity GUI?")]
		public ReportTypes reportType = ReportTypes.GUI;

		[Tooltip("Normalized viewport rectangle describing the area the speed appears in, values between 0 and 1")]
		public Rect guiArea;

		private Vector3 lastPosition;
		private float spd = 0f;
		[RequiredFieldAttribute("Reference to the optional rigidbody we are reporting on (works for other types of motion as well)")]
		public Rigidbody body;

		[Tooltip("If false, normal update will be used instead")]
		public bool useFixedUpdate = true;

		public HelpInfo help = new HelpInfo("This component gives you the current speed of an object. If using the legacy GUI setting, not suitable for mobile. Debug mode is slow," +
			" so on mobile we recommend using Unity's new UI system to display the output instead.");

		void OnEnable () {
			body = GetComponent<Rigidbody>(); 
			lastPosition = transform.position;
		}

		void Update () {
			if (useFixedUpdate)
				return;

			spd = Vector3.Distance(transform.position, lastPosition) * Time.deltaTime;

			lastPosition = transform.position;

			if (reportType == ReportTypes.Debug)
				Debug.Log("Speed: " + spd);
		}

		void FixedUpdate () {
			if (body == null) {
				Debug.LogWarning("Speed Reporter " + gameObject.name + " needs a rigidbody for Fixed Update mode!");
				useFixedUpdate = false;
			}

			if (useFixedUpdate)
				spd = body.velocity.magnitude;
			if (reportType == ReportTypes.Debug)
				Debug.Log("Speed of " + gameObject.name + ": " + spd);
		}

		void OnGUI () {
			if (reportType == ReportTypes.GUI){
				GUILayout.BeginArea(new Rect(Screen.width * guiArea.x, Screen.height * guiArea.y, Screen.width *  guiArea.width, Screen.height * guiArea.height),"","box");
				GUILayout.Label("Speed: " + spd);
				GUILayout.EndArea();
			}
		}
	}
}