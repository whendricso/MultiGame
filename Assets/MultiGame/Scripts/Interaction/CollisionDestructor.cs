using UnityEngine;
using System.Collections;

public class CollisionDestructor : MonoBehaviour {

	public bool destroySelf = false;
	public bool destroyOther = true;

	void OnCollisionEnter (Collision _collision) {
		if (destroyOther)
			Destroy(_collision.gameObject);
		if (destroySelf)
			Destroy(gameObject);
	}

}
