using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent (typeof(Rigidbody))]
	public class Hover : MultiModule {

		[Tooltip("How high can we go before hover thrust stops?")]
		public float rayDistance = 6.0f;
		[Tooltip("How far should the hover check ray be offset from origin?")]
		public Vector3 rayOffset;
		[Tooltip("Which objects can we hover over?")]
		public LayerMask hoverRayMask;
		[Tooltip("How much upward thrust is applied by this hover motor?")]
		public float hoverForce = 5.0f;

		public HelpInfo help = new HelpInfo("This component implements physics-based hovering. It raycasts down into the scene to find out if we are above something, and if so" +
			" it applies upward force.");

		public bool debug = false;

		void FixedUpdate () {
			RaycastHit hinfo;
			bool didHit = Physics.Raycast(transform.position + rayOffset, -Vector3.up, out hinfo, rayDistance, hoverRayMask);
			if (didHit)
				GetComponent<Rigidbody>().AddForce(Vector3.up * hoverForce, ForceMode.Force);

			if (debug) {
				Debug.DrawRay(transform.position + rayOffset, -Vector3.up);
				Debug.Log("" + didHit);
			}
		}
	}
}