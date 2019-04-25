using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Physics Toggle")]
[RequireComponent (typeof(Rigidbody))]
	public class PhysicsToggle : MultiModule {

		[Tooltip("Do we also affect gravity?")]
		public bool toggleGravity = true;
		[Tooltip("Is there a delay when toggling?")]
		public float delay = 0.0f;
		[Tooltip("Should we force any movement to stop?")]
		public bool nullifyMotion = false;
		[Tooltip("Should this object control the 'Convex' state of the collider also? This will force non-convex colliders to become convex if they need to move. But be careful, this can cause physics glitches in some circumstances!")]
		public bool controlColliderState = false;
	//	public float brakingDrag = 10.0f;

		MeshCollider _coll/* = GetComponent<MeshCollider>()*/;
	//	private float originalDrag;
	//	private float originalAngular;

		private Rigidbody rigid;

		public HelpInfo help = new HelpInfo("This component allows the physics simulation of a given object to be toggled based on messages. 'EnablePhysics' and 'DisablePhysics' " +
			"take no parameters. 'SwapPhysics' takes no parameter and will reverse the physics state of the object. It also optionally changes the 'convex' state of a collider based on physics state (recommended for advanced users only) " +
			"\n\n" +
			"To use, add to a physics object and when appropriate, send messages to this component using any message sender (such as ActiveCollider).");

		void OnEnable () {
	//		originalDrag = GetComponent<Rigidbody>().drag;
	//		originalAngular = GetComponent<Rigidbody>().angularDrag;
	
			if (_coll == null)
				_coll = GetComponent<MeshCollider>();
			if (rigid == null)
				rigid = GetComponent<Rigidbody> ();
			if (_coll != null) {
				if (rigid.isKinematic)
					_coll.convex = false;
				else
					_coll.convex = true;
			}
		}

		void FixedUpdate () {
			if (nullifyMotion) {
				rigid.velocity = Vector3.zero;
				rigid.angularVelocity = Vector3.zero;
			}
		}

		[Header("Available Messages")]
		public MessageHelp disablePhysicsHelp = new MessageHelp("DisablePhysics","Disables physics calculations on this object (it will still collide with objects if it has a collider)");
		public void DisablePhysics () {
			StartCoroutine( TogglePhysics(false));
			if (_coll != null)
				_coll.convex = true;
		}

		public MessageHelp enablePhysicsHelp = new MessageHelp("EnablePhysics","Disables physics calculations on this object");
		public void EnablePhysics () {
			if (!gameObject.activeInHierarchy)
				return;
			StartCoroutine( TogglePhysics(true));
			if (_coll != null)
				_coll.convex = true;
		}

		IEnumerator TogglePhysics(bool val) {
			yield return new WaitForSeconds(delay);
			if (!val && nullifyMotion)
				rigid.velocity = Vector3.zero;
			if (toggleGravity)
				rigid.useGravity = val;
			rigid.isKinematic = !val;
			if (_coll != null)
				_coll.convex = val;
			if (val)
				rigid.WakeUp();
		}

		public MessageHelp swapPhysicsHelp = new MessageHelp("SwapPhysics","Toggles the physics state of this object.");
		public void SwapPhysics () {
			if (!gameObject.activeInHierarchy)
				return;
			if (!rigid.isKinematic)
				StartCoroutine( TogglePhysics(false));
			else
				StartCoroutine( TogglePhysics(true));
		}

	}
}