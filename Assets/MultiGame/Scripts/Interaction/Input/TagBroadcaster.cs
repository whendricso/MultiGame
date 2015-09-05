using UnityEngine;
using System.Collections;

public class TagBroadcaster : MultiModule {

	[Tooltip("Tag associated with objects we want to talk to")]
	public string targetTag = "";
	[Tooltip("The message we want to send those objects")]
	public MessageManager.ManagedMessage message;

	[Tooltip("Should we auto-broadcast when we are created?")]
	public bool onStart = false;
	[Tooltip("Should we auto-broadcast every single frame?")]
	public bool onUpdate = false;

	public HelpInfo help = new HelpInfo("This component sends messages to all objects of a given tag. Can activate automatically, or based on other message senders with " +
		"'Broadcast'");

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
