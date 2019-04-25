using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Hover")]
	[RequireComponent (typeof(Rigidbody))]
	public class Hover : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("Which objects can we hover over?")]
		public LayerMask hoverRayMask;
		[Header("Hover Settings")]
		[RequiredFieldAttribute("How high can we go before hover thrust stops?")]
		public float rayDistance = 6.0f;
		[Tooltip("How far should the hover check ray be offset from origin?")]
		public Vector3 rayOffset;
		[Tooltip("How much upward thrust is applied by this hover motor?")]
		public float hoverForce = 5.0f;

		public HelpInfo help = new HelpInfo("This component implements physics-based hovering. It raycasts down into the scene to find out if we are above something, and if so" +
			" it applies upward force.");


		private Rigidbody rigid;

		public bool debug = false;

		void OnEnable () {
			if (rigid == null)
				rigid = GetComponent<Rigidbody>();
		}

		RaycastHit hinfo;
		void FixedUpdate () {
			bool didHit = Physics.Raycast(transform.position + rayOffset, -Vector3.up, out hinfo, rayDistance, hoverRayMask);
			if (didHit)
				rigid.AddForce(Vector3.up * hoverForce, ForceMode.Force);

			if (debug) {
				Debug.DrawRay(transform.position + rayOffset, -Vector3.up);
				Debug.Log("" + didHit);
			}
		}
	}
}