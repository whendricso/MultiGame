using UnityEngine;
using System.Collections;

public class CursorLocker : MonoBehaviour {
	
	public KeyCode breakMode = KeyCode.LeftControl;
	public MouseLook[] mouseLooks;
	
	void Start () {
		if (mouseLooks.Length == 0) {
			mouseLooks = gameObject.GetComponentsInChildren<MouseLook>();
		}
		Screen.lockCursor = true;
	}
	
	void Update () {
		if (Input.GetKeyDown(breakMode)) {
			if (mouseLooks.Length > 0) {
				foreach (MouseLook mouseLook in mouseLooks) {
					mouseLook.enabled = false;
				}
			}
			Screen.lockCursor = false;
		}
		if (Input.GetKeyUp(breakMode)) {
			if (mouseLooks.Length > 0) {
				foreach (MouseLook mouseLook in mouseLooks) {
					mouseLook.enabled = true;
				}
			}
			Screen.lockCursor = true;
		}
	}
}
