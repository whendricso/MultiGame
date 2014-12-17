using UnityEngine;
using System.Collections;

public class DestroyWithMe : MonoBehaviour {

	public GameObject[] targets;

	void OnDestroy () {
		if (targets.Length > 0) {
			foreach(GameObject target in targets) {
				if (target != null)
					Destroy(target);
			}
		}
	}

}
