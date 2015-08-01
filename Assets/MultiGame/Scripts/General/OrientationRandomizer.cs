using UnityEngine;
using System.Collections;

public class OrientationRandomizer : MonoBehaviour {

	void Start () {
		transform.RotateAround(transform.position, Vector3.up, Random.Range(0,360));
	}
}
