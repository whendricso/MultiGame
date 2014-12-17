using UnityEngine;
using System.Collections;

public class StickToGround : MonoBehaviour {
	
	public float maxDistance = 0.5f;
	public Vector3 offset = Vector3.zero;
	public bool onStart = true;
	public bool everyFrame = true;
	
	void Start () {
		if (onStart)
			Adhere();
	}
	
	void LateUpdate () {
		if (everyFrame)
			Adhere();
	}
	
	private void Adhere () {
		RaycastHit hinfo;
		bool didHit = Physics.Raycast(transform.position, Vector3.down, out hinfo, maxDistance);
		if (didHit)
			transform.position = hinfo.point + offset;
	}
}
