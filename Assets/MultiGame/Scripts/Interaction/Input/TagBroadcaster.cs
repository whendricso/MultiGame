using UnityEngine;
using System.Collections;

public class TagBroadcaster : MonoBehaviour {

	public string targetTag = "";
	public MessageManager.ManagedMessage message;

	public bool onStart = false;
	public bool onUpdate = false;

	void Start () {
		if (string.IsNullOrEmpty( message.message))
			message.message = "Activate";//Activate is the global default message
		if (onStart) {
			Broadcast();
		}
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
	}

	void Update () {
		if (onUpdate) {
			Broadcast();
		}
	}

	public void Broadcast () {
		if (message.target != null)
			MessageManager.SendTo(message, message.target);
		else {
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag(targetTag))
				MessageManager.SendTo(message, obj);
		}
	}
}
