using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class PhysicsToggle : MonoBehaviour {

	public bool toggleGravity = true;
	public float delay = 0.0f;
	public bool nullifyMotion = false;
	public float brakingDrag = 10.0f;

	MeshCollider _coll/* = GetComponent<MeshCollider>()*/;
	private float originalDrag;
	private float originalAngular;

	void Start () {
		originalDrag = rigidbody.drag;
		originalAngular = rigidbody.angularDrag;
		_coll = GetComponent<MeshCollider>();
		if (_coll != null) {
			if (rigidbody.isKinematic)
				_coll.convex = false;
			else
				_coll.convex = true;
		}
	}

	void FixedUpdate () {
		if (nullifyMotion) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
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
			rigidbody.velocity = Vector3.zero;
		if (toggleGravity)
			rigidbody.useGravity = val;
		rigidbody.isKinematic = !val;
		if (_coll != null)
			_coll.convex = val;
		if (val)
			rigidbody.WakeUp();
	}

	public void SwapPhysics () {
		if (!rigidbody.isKinematic)
			StartCoroutine( TogglePhysics(false));
		else
			StartCoroutine( TogglePhysics(true));
	}

}
