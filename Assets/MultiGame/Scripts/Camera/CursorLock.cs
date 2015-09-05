using UnityEngine;
using System.Collections;

public class CursorLock : MultiModule {

	[Tooltip("List of keys that unlock the mouse")]
	public KeyCode[] unLockKeys = new KeyCode[] {KeyCode.Escape, KeyCode.LeftControl};
	[Tooltip("List of keys that lock the mouse")]
	public KeyCode[] lockKeys = new KeyCode[] {KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D};
	[System.NonSerialized]
	public bool lockOnClick = false;

	public HelpInfo help = new HelpInfo("This component allows the cursor to hide/unhide based on messages sent from other components, or just when the player clicks on the game.");

	void Update () {
		if (lockOnClick)
			LockOnClick();

		foreach (KeyCode _kc in lockKeys) {
			if (Input.GetKey(_kc))
				LockMouse();
		}

		foreach (KeyCode _kc in unLockKeys) {
			if (Input.GetKey(_kc))
				UnlockMouse();
		}
	}

	void LockOnClick () {
		for (int i = 0; i < 3; i++)
		if (Input.GetMouseButton(i)) {
			LockMouse();
		}
	}

	public void LockMouse () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void UnlockMouse () {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}