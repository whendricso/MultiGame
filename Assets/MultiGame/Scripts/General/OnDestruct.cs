using UnityEngine;
using System.Collections;

public class OnDestruct : MonoBehaviour {

	public GameObject target;
	public string message = "";

	void Start () {
		if (target == null)
			target = gameObject;
	}

	void OnDestroy () {
		if (message == "")
			return;

		target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
	}
}
