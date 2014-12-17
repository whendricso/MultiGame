using UnityEngine;
using System.Collections;

public class PlanarYDestructor : MonoBehaviour {
	
	public float minimumYLevel = -1000;//destroy the object if it falls below this plane
	
	void Update () {
		if (transform.position.y <= minimumYLevel)
			Destroy(gameObject);
	}
}
