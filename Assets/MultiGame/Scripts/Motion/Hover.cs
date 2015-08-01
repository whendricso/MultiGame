using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Hover : MonoBehaviour {

	public float rayDistance = 6.0f;
	public Vector3 rayOffset;
	public LayerMask hoverRayMask;
	public float hoverForce = 5.0f;

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
