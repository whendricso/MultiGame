using UnityEngine;
using System.Collections;

public class OnDestruct : MultiModule {

	[Tooltip("Optional message target override")]
	public GameObject target;
	[Tooltip("Message to be sent when this object is destroyed")]
	public MessageManager.ManagedMessage message;
	public HelpInfo help = new HelpInfo("This component allows messages to be sent when an object is destroyed.");
	

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
