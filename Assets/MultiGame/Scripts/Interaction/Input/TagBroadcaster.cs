using UnityEngine;
using System.Collections;

public class TagBroadcaster : MonoBehaviour {

	public string targetTag = "";
	public MessageManager.ManagedMessage message;

	public bool onStart = true;
	public bool onUpdate = false;

	void Start () {
		if (string.IsNullOrEmpty( message.message))
			message.message = "Activate";//Activate is the global default message
		if (onStart) {
			Broadcast();
		}
	}
	
	void Update () {
		if (onUpdate) {
			Broadcast();
		}
	}

	void Broadcast () {
		if (message.target != null)
			MessageManager.SendTo(message, message.target);
		else {
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag(targetTag))
				MessageManager.SendTo(message, obj);
		}
	}
}
