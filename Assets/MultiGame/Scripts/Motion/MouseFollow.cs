using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Mouse Follow")]
	public class MouseFollow : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("What kinds of things can the mouse pass onto?")]
		public LayerMask layerMask;

		[Header("Constraint Settings")]
		[RequiredFieldAttribute("How far away can the mouse be at most?")]
		public float maxDistance = 1500.0f;
		[Tooltip("Should my position be constrained on the X axis?")]
		public bool constrainX = false;
		private float originalX = 0;
		[Tooltip("Should my position be constrained on the Y axis?")]
		public bool constrainY = false;
		private float originalY = 0;
		[Tooltip("Should my position be constrained on the Z axis?")]
		public bool constrainZ = false;
		private float originalZ = 0;

		public HelpInfo help = new HelpInfo("This component causes an object to always be at the position of the mouse in the world. To use, add it to an object that you want to follow the mouse. Then, populate the Layer Mask at the " +
			"top with layers of objects that represent the world geometry, such as the terrain. Then, this object will always be under the player's cursor while it's alive, as long as the cursor is over one of these object groups. " +
			"This can be useful for selection painting, object placement, spellcasting, or creating a light around the cursor in dimly-lit game areas.");

		void OnEnable () {
			originalX = transform.position.x;
			originalY = transform.position.y;
			originalZ = transform.position.z;
		}
		
		void Update () {
			RaycastHit hinfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			bool didHit = Physics.Raycast(ray, out hinfo, maxDistance, layerMask);
			if (didHit) {
				transform.position = hinfo.point;
				if (constrainX)
					transform.position = new Vector3(originalX, transform.position.y, transform.position.z);
				if (constrainY)
					transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
				if (constrainZ)
					transform.position = new Vector3(transform.position.x, transform.position.y, originalZ);
			}
		}
	}
}
