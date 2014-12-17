using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	
	public Transform target;
	
	void Update () {
		if(target == null && Camera.main != null) {
			target = Camera.main.GetComponent<Transform>();
			return;
		}
		
		if (target != null)
			transform.LookAt(target.transform, Vector3.up);
	}
}
