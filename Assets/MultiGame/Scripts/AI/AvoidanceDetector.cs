using UnityEngine;
using System.Collections;

public class AvoidanceDetector : MonoBehaviour {

	public GameObject left;
	public GameObject right;
	public GameObject center;

	public LayerMask avoidRayMask;

	public float sideRayLength = 4.5f;
	public float centerRayLength = 3.0f;

	[System.NonSerialized]
	public bool leftDetected = false;
	[System.NonSerialized]
	public bool rightDetected = false;
	[System.NonSerialized]
	public bool centerDetected = false;

	public bool debug = false;

	void Awake () {
		if((left == null || right == null) || center == null) {
			Debug.LogError("Avoidance Detector " + gameObject.name + " needs left, right, and center ray cast transforms assigned in the inspector.");
			enabled = false;
			return;
		}

	}

	void FixedUpdate () {
		leftDetected = CheckLeft();
		rightDetected = CheckRight();
		centerDetected = CheckCenter();

		if (debug) {
			if ((leftDetected || rightDetected) || centerDetected)
				Debug.Log("Avoidance Detector " + gameObject.name + " is avoiding an object collision.");
		}
	}

	#region rayCheks
	bool CheckLeft () {
		bool ret = false;

		if (Physics.Raycast(left.transform.position, left.transform.forward, sideRayLength, avoidRayMask))
			ret = true;

		return ret;
	}

	bool CheckRight () {
		bool ret = false;

		if (Physics.Raycast(right.transform.position, right.transform.forward, sideRayLength, avoidRayMask))
			ret = true;

		return ret;
	}

	bool CheckCenter () {
		bool ret = false;

		if (Physics.Raycast(center.transform.position, center.transform.forward, centerRayLength, avoidRayMask))
			ret = true;

		return ret;
	}
	#endregion
}
