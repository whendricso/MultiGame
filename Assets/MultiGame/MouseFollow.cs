using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {
	
	public float maxDistance = 1500.0f;
	public LayerMask layerMask;
	
	void Update () {
		RaycastHit hinfo;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool didHit = Physics.Raycast(ray, out hinfo, maxDistance, layerMask);
		if (didHit)
			transform.position = hinfo.point;
	}
}
