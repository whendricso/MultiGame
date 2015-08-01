using UnityEngine;
using System.Collections;

public class OnDestruct : MonoBehaviour {

	public GameObject target;
	public MessageManager.ManagedMessage message;

	void Start () {
		if (target == null)
			target = gameObject;
		if (message.target == null)
			message.target = target;
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
	}

	void OnDestroy () {
		if (message.message == "")
			return;

		MessageManager.Send(message);
	}
}
