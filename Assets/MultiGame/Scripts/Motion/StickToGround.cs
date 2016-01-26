using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class StickToGround : MultiModule {
		
		[Tooltip("How high up can it work?")]
		public float maxDistance = 0.5f;
		[Tooltip("The offset relative to the point we found we should move to")]
		public Vector3 offset = Vector3.zero;
		[Tooltip("Should we do this as soon as we spawn?")]
		public bool onStart = true;
		[Tooltip("Should we do this constantly?")]
		public bool everyFrame = true;

		public HelpInfo help = new HelpInfo("This component causes objects (such as buildings) to snap to the object underneath using a raycast");
		
		void Start () {
			if (onStart)
				Adhere();
		}
		
		void LateUpdate () {
			if (everyFrame)
				Adhere();
		}
		
		private void Adhere () {
			RaycastHit hinfo;
			bool didHit = Physics.Raycast(transform.position, Vector3.down, out hinfo, maxDistance);
			if (didHit)
				transform.position = hinfo.point + offset;
		}
	}
}