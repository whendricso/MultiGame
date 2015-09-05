using UnityEngine;
using System.Collections;

public class StartMessage : MultiModule {

	[Tooltip("When this object is created, what message should we send?")]
	public MessageManager.ManagedMessage[] messages;

	public HelpInfo help = new HelpInfo("This component sends messages as soon as the object is created.");

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
