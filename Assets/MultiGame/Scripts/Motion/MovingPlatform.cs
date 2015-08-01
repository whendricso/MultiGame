using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour {

	[System.NonSerialized]
	private List<GameObject> others = new List<GameObject>();

	Vector3 lastPos;

	void Start () {
		lastPos = transform.position;
	}

	void OnCollisionEnter(Collision _collision) {
		if (others.Contains(_collision.gameObject))
			return;

		others.Add(_collision.gameObject);
	}

	void OnCollisionExit(Collision _collision) {
		if (others.Contains(_collision.gameObject))
			others.Remove(_collision.gameObject);
	}

	void FixedUpdate() {
		foreach(GameObject gobj in others) {
			gobj.transform.Translate(transform.position - lastPos, Space.World);
		}

		lastPos = transform.position;
	}

}
