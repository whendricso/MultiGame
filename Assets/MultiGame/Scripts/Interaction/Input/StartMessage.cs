using UnityEngine;
using System.Collections;

public class StartMessage : MonoBehaviour {

	public MessageManager.ManagedMessage[] messages;

	void Start () {
		foreach (MessageManager.ManagedMessage _message in messages) {
			MessageManager.Send(_message);
		}
	}

}
