using UnityEngine;
using System.Collections;

public class Clickable : MonoBehaviour {
	
	public MessageManager.ManagedMessage message;
	public bool asButton = true;
	public bool debug = false;
	public KeyCode modifier = KeyCode.None;
	
	void Start () {
		if (message.target == null) {
			message.target = gameObject;
		}
	}
	
	void OnMouseUpAsButton() {
		if (asButton)
			ClickMessage();
	}

	void OnMouseDown () {
		if (!asButton)
			ClickMessage();
	}

	void ClickMessage () {
		if (modifier != KeyCode.None && !Input.GetKey(modifier)) {
			if (debug)
				Debug.Log("Click discarded");
			return;
		}
		if (debug)
			Debug.Log("Clicked " + gameObject.name);
		MessageManager.Send(message);//target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
	}

}