using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageRelay : MonoBehaviour {

	public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

	void Start () {
		if (messages.Count < 1) {
			Debug.LogError("Message Relay " + gameObject.name + " needs a list of messages!");
			enabled = false;
			return;
		}

		foreach(MessageManager.ManagedMessage msg in messages) {
			if (msg.target == null)
				msg.target = gameObject;
		}
	}

	void OnValidate () {
		for (int i = 0; i < messages.Count; i++) {
			MessageManager.ManagedMessage _msg = messages[i];
			MessageManager.UpdateMessageGUI(ref _msg, gameObject);
		}
	}
	
	public void Relay () {
		if (enabled == false)
			return;

		foreach(MessageManager.ManagedMessage msg in messages) {
			MessageManager.Send(msg);
		}
	}
}
