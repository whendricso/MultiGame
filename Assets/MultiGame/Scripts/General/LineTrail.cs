using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LineTrail : MonoBehaviour {

	private Vector3 startPosition;
	[System.NonSerialized]
	public LineRenderer line;

	void Start () {
		line = GetComponent<LineRenderer>();
		startPosition = transform.position;
	}
	
	void Update () {
		line.SetPosition(0, startPosition);
		line.SetPosition(1, this.transform.position);
	}
}
