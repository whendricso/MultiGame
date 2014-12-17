using UnityEngine;
using System.Collections;

public class KeyMessage : MonoBehaviour {

	public KeyCode key = KeyCode.None;
	public MessageManager.ManagedMessage keyDownMessage;
	public MessageManager.ManagedMessage keyHeldMessage;
	public MessageManager.ManagedMessage keyUpMessage;

	public GameObject target;

	void Start () {
		if (target == null)
			target = gameObject;
		if (keyDownMessage.target == null)
			keyDownMessage.target = target;
		if (keyUpMessage.target == null)
			keyUpMessage.target = target;
		if (keyHeldMessage.target == null)
			keyHeldMessage.target = target;
	}

	void Update () {
		if (Input.GetKeyDown(key) && keyDownMessage.message != "") {
			MessageManager.Send (keyDownMessage);
		}
		if (Input.GetKey(key) && keyHeldMessage.message != "") {
			MessageManager.Send (keyHeldMessage);
		}
		if (Input.GetKeyUp(key) && keyUpMessage.message != "") {
			MessageManager.Send (keyUpMessage);
		}
	}
}
