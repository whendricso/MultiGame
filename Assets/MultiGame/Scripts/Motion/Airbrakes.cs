using UnityEngine;
using System.Collections;

public class Airbrakes : MonoBehaviour {

	public KeyCode brake = KeyCode.S;
	public Rigidbody myRigidbody;

	public float brakingDrag = 10.0f;
	[System.NonSerialized]
	private float originalDrag;

	// Use this for initialization
	void Start () {
		if (myRigidbody == null)
			myRigidbody = GetComponent<Rigidbody>();
		if (myRigidbody == null) {
			Debug.LogError("Airbrakes " + gameObject.name + "needs a rigidbody assigned!");
			enabled = false;
			return;
		}
		originalDrag = myRigidbody.drag;

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(brake) || Input.GetAxis("Vertical") < -0.8f)
			myRigidbody.drag = brakingDrag;
		else
			myRigidbody.drag = originalDrag;
	}
}
