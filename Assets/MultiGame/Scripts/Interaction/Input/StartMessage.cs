using UnityEngine;
using System.Collections;

public class StartMessage : MonoBehaviour {

	public MessageManager.ManagedMessage[] messages;

	void Start () {
		foreach (MessageManager.ManagedMessage _message in messages) {
			MessageManager.Send(_message);
		}
	}

	void OnValidate () {
		for (int i = 0; i < this.messages.Length; i++) {
			MessageManager.ManagedMessage _msg = messages[i];
			MessageManager.UpdateMessageGUI(ref _msg, gameObject);
		}
	}
}
