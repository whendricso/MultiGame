using UnityEngine;
using System.Collections;

public class MessageParentRelation : MonoBehaviour {

	public GameObject target;
	public GameObject targetParent;

	void Start () {
		if (target == null)
			target = gameObject;
	}
	
	void Parent () {
		target.transform.parent = targetParent.transform;
	}

	void Unparent () {
		target.transform.parent = null;
	}

	void SetParent (GameObject newParent) {
		target.transform.parent = newParent.transform;
	}

	void ToggleParent () {
		if (transform.parent == null)
			transform.parent = targetParent.transform;
		else
			transform.parent = null;
	}

	void Activate () {
		Unparent();
	}
}
