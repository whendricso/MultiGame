using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class PhysicsToggle : MultiModule {

	[Tooltip("Do we also affect gravity?")]
	public bool toggleGravity = true;
	[Tooltip("Is there a delay when toggling?")]
	public float delay = 0.0f;
	[Tooltip("Should we force any movement to stop?")]
	public bool nullifyMotion = false;
//	public float brakingDrag = 10.0f;

	MeshCollider _coll/* = GetComponent<MeshCollider>()*/;
//	private float originalDrag;
//	private float originalAngular;

	public HelpInfo help = new HelpInfo("This component allows the physics simulation of a given object to be toggled based on messages.");

	void Start () {
//		originalDrag = GetComponent<Rigidbody>().drag;
//		originalAngular = GetComponent<Rigidbody>().angularDrag;
		_coll = GetComponent<MeshCollider>();
		if (_coll != null) {
			if (GetComponent<Rigidbody>().isKinematic)
				_coll.convex = false;
			else
				_coll.convex = true;
		}
	}

	void FixedUpdate () {
		if (nullifyMotion) {
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
	}

	public void DisablePhysics () {
		StartCoroutine( TogglePhysics(false));
		if (_coll != null)
			_coll.convex = true;
	}

	public void EnablePhysics () {
		StartCoroutine( TogglePhysics(true));
		if (_coll != null)
			_coll.convex = true;
	}

	public IEnumerator TogglePhysics(bool val) {
		yield return new WaitForSeconds(delay);
		if (!val && nullifyMotion)
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		if (toggleGravity)
			GetComponent<Rigidbody>().useGravity = val;
		GetComponent<Rigidbody>().isKinematic = !val;
		if (_coll != null)
			_coll.convex = val;
		if (val)
			GetComponent<Rigidbody>().WakeUp();
	}

	public void SwapPhysics () {
		if (!GetComponent<Rigidbody>().isKinematic)
			StartCoroutine( TogglePhysics(false));
		else
			StartCoroutine( TogglePhysics(true));
	}

}
