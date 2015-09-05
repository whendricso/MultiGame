using UnityEngine;
using System.Collections;

public class RandomSpin : MultiModule {

	[Tooltip("Do we apply rotation just once, or continually?")]
	public bool oneShot = false;
	private bool didStart = false;
	[Tooltip("")]
	public float power = 10.0f;
	public bool randomizeStartRotation = false;

	public HelpInfo help = new HelpInfo("This component randomly spins the object at runtime on all axes");

	void Start() {
		if (randomizeStartRotation) {
			transform.rotation = Random.rotation;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (GetComponent<Rigidbody>().isKinematic || GetComponent<Rigidbody>() == null) {
			if (oneShot && didStart) 
				return;
			didStart = true;
			transform.RotateAround(transform.position, Vector3.Normalize( Random.rotation.eulerAngles), Random.Range(-power, power));
		}
		else {
			if (oneShot && didStart) 
				return;
			didStart = true;
				GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.Range(-power, power),Random.Range(-power, power),Random.Range(-power, power)));
			
		}
	}
}
