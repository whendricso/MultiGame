using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	public bool instant = true;
	public Transform target;
	public float speed = 6.0f;
	
	void Update () {
		if(target == null && Camera.main != null) {
			target = Camera.main.GetComponent<Transform>();
			return;
		}
		
		if (target != null) {
			if (instant)
				transform.LookAt(target.transform, Vector3.up);
			else
				transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.position - transform.position, speed * Time.deltaTime,0f));
		}
	}
}