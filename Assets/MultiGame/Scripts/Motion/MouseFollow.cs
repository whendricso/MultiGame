using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {
	
	public float maxDistance = 1500.0f;
	public LayerMask layerMask;
	public bool constrainX = false;
	private float originalX = 0;
	public bool constrainY = false;
	private float originalY = 0;
	public bool constrainZ = false;
	private float originalZ = 0;

	void Awake () {
		originalX = transform.position.x;
		originalY = transform.position.y;
		originalZ = transform.position.z;
	}
	
	void Update () {
		RaycastHit hinfo;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool didHit = Physics.Raycast(ray, out hinfo, maxDistance, layerMask);
		if (didHit) {
			transform.position = hinfo.point;
			if (constrainX)
				transform.position = new Vector3(originalX, transform.position.y, transform.position.z);
			if (constrainY)
				transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
			if (constrainZ)
				transform.position = new Vector3(transform.position.x, transform.position.y, originalZ);
		}
	}
}
