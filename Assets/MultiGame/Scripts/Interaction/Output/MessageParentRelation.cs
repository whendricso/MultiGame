using UnityEngine;
using System.Collections;

public class MessageParentRelation : MonoBehaviour {

	public GameObject target;
	public GameObject targetParent;

	void Start () {
		if (target == null)
			target = gameObject;
	}
	
	public void Parent () {
		target.transform.parent = targetParent.transform;
	}

	public void Unparent () {
		target.transform.parent = null;
	}

	public void SetParent (GameObject newParent) {
		target.transform.parent = newParent.transform;
	}

	public void ToggleParent () {
		if (transform.parent == null)
			transform.parent = targetParent.transform;
		else
			transform.parent = null;
	}

	public void Activate () {
		Unparent();
	}
}
