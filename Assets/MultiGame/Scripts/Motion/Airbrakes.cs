using UnityEngine;
using System.Collections;

public class Airbrakes : MultiModule {

	[Tooltip("Which key activates the brakes when held?")]
	public KeyCode brake = KeyCode.S;
	[Tooltip("Which rigidbody are we affecting?")]
	public Rigidbody myRigidbody;

	[Tooltip("What should the drag be changed to while air braking?")]
	public float brakingDrag = 10.0f;
	[System.NonSerialized]
	private float originalDrag;

	public HelpInfo help = new HelpInfo("This component allows a physics object to slow down in midair using drag based on the state of a key.");

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
		if(Input.GetKey(brake))
			myRigidbody.drag = brakingDrag;
		else
			myRigidbody.drag = originalDrag;
	}
}
