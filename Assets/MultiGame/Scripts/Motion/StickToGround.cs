using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Stick To Ground")]
	public class StickToGround : MultiModule {
		
		[RequiredFieldAttribute("How high up can it work?")]
		public float maxDistance = 0.5f;
		[Tooltip("The offset relative to the point we found we should move to")]
		public Vector3 offset = Vector3.zero;
		[Tooltip("Should we do this as soon as we spawn?")]
		public bool onStart = true;
		[Tooltip("Should we do this constantly?")]
		public bool everyFrame = true;
		[Tooltip("Should we automatically parent to the object we snap to?")]
		public bool autoParent = false;

		public HelpInfo help = new HelpInfo("This component causes objects (such as buildings) to snap to the object underneath using a raycast. It works by raycasting down from the origin of this object, and snaps that origin to " +
			"the first collider it hits.");
		
		void Start () {
			if (onStart)
				Adhere();
		}
		
		void LateUpdate () {
			if (everyFrame)
				Adhere();
		}

		public MessageHelp adhereHelp = new MessageHelp ("Adhere","Immediately snaps this object to what ever is below, based on the settings above.");
		public void Adhere () {
			RaycastHit hinfo;
			bool didHit = Physics.Raycast(transform.position, Vector3.down, out hinfo, maxDistance);
			if (didHit) {
				transform.position = hinfo.point + offset;
				if (autoParent)
					transform.SetParent (hinfo.collider.transform);
			}
		}
	}
}